using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RateLimiter;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   public class VkApiResponseInfo {
      public VkApiResponseInfo(string response, DateTimeOffset storedAt) {
         Response = response;
         StoredAt = storedAt;
      }

      public Uri Uri { get; set; }
      public string Response { get; private set; }
      public DateTimeOffset StoredAt { get; private set; }
   }

   [UsedImplicitly]
   public class VkApi {
      public const string VERSION = "5.62";

      private readonly HttpClient _httpClient;
      private readonly AccessToken _token;
      private readonly ConcurrentDictionary<string, VkApiResponseInfo> _responseCache;
      private readonly ConcurrentDictionary<string, DateTimeOffset> _requestLog;

      private readonly TimeLimiter _rateLimiter;
      private int _tooMuchRequestsOccurrences;
      private int _timeoutRetry;

      public event EventHandler<VkApiResponseInfo> CallPerformed;

      public VkApi(AccessToken token, HttpMessageHandler httpClientHandler) {
         _token = token;
         _httpClient = new HttpClient(httpClientHandler) {Timeout = TimeSpan.FromSeconds(4)};
         _responseCache = new ConcurrentDictionary<string, VkApiResponseInfo>();
         _requestLog = new ConcurrentDictionary<string, DateTimeOffset>();

         _rateLimiter = TimeLimiter.GetFromMaxCountByInterval(3, TimeSpan.FromSeconds(1.00f));
      }

      private void checkForErrors(string response) {
         if (string.IsNullOrEmpty(response) || response.Length < 5) {
            throw new VkException($"Не получен ответ от сервера");
         }

         if (response.Substring(2, 5) == "error") {
            var error = deserializeError(response);
            Debug.WriteLine($"Vk error code: {error.ErrorCode}. {error.ErrorMessage}");
            throw new VkException($"Код ошибки: {error.ErrorCode}\n{error.ErrorMessage}", error.ErrorCode);
         }
      }
      
      public async Task<string> ExecuteMethodAsync(string method, [NotNull] QueryParameters query, CancellationToken ct) {
         if (query == null) {
            throw new ArgumentNullException(nameof(query));
         }

         var uri = buildCompleteUri(method, query);

         var key = uri.ToString();

         //защита от множества запросов за короткое время, что бы они возможно получили данные из кэша
         if (_requestLog.ContainsKey(key)) {
            DateTimeOffset lastLog;
            if (_requestLog.TryGetValue(key, out lastLog)) {
               var timeSpan = DateTimeOffset.Now - lastLog;
               if (timeSpan < TimeSpan.FromSeconds(1)) {
                  await Task.Delay(TimeSpan.FromSeconds(1), ct);
               }
               else {
                  _requestLog.TryRemove(key, out lastLog);
               }
            }
         }
         else {
            _requestLog.TryAdd(key, DateTimeOffset.Now);
         }

         if (_responseCache.ContainsKey(key)) {
            VkApiResponseInfo responseInfo;
            var result = _responseCache.TryGetValue(key, out responseInfo);
            if (result) {
               var timeSpan = DateTimeOffset.Now - responseInfo.StoredAt;
               if (timeSpan < TimeSpan.FromSeconds(5)) {
                  return responseInfo.Response;
               }
               else {
                  _responseCache.TryRemove(key, out responseInfo);
               }
            }
         }

         return await _rateLimiter.Perform(() => callAsync(uri, ct), ct).ConfigureAwait(false);
      }

      public async Task<string> ExecuteMethodIgnoreCacheAsync(string method, [NotNull] QueryParameters query, CancellationToken ct) {
         if (query == null) {
            throw new ArgumentNullException(nameof(query));
         }

         var uri = buildCompleteUri(method, query);
         return await _rateLimiter.Perform(() => callAsync(uri, ct), ct).ConfigureAwait(false);
      }

      private async Task<string> callAsync(Uri uri, CancellationToken ct) {
         try {
            var response = await _httpClient.GetAsync(uri, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            checkForErrors(result);

            var vkResponse = new VkApiResponseInfo(result, DateTimeOffset.Now) {
               Uri = uri
            };

            OnCallPerformed(vkResponse);

            return result;
         }
         catch (HttpRequestException ex) {
            throw new VkException($"Соеденение не удалось.\nПроверьте настройки прокси сервера и перезапустите приложение.\n{ex.Message}", ex);
         }
         catch (WebException ex) {
            if (tryToHandleException(ex) == false) {
               throw;
            }
         }
         catch (VkException ex) {
            if (_tooMuchRequestsOccurrences > 2) {
               throw;
            }

            _tooMuchRequestsOccurrences++;
            //too much requests per second
            if (ex.ErrorCode == 6) {
               await Task.Delay(TimeSpan.FromSeconds(1f), ct);
               if (!ct.IsCancellationRequested) {
                  return await callAsync(uri, ct);
               }
            }

            throw;
         }
         // Httpclient timeout
         catch (TaskCanceledException ex) {
            if (ct.IsCancellationRequested) {
               throw;
            }

            if (_timeoutRetry > 1) {
               throw new VkException($"Соеденение не удалось.\nПроверьте настройки прокси сервера и перезапустите приложение.\n{ex.Message}", ex);
            }
            _timeoutRetry++;
            return await callAsync(uri, ct).ConfigureAwait(false);
         }
         //catch (VkException ex) {
         //   //captcha needed
         //   if (ex.ErrorCode == 14) {
         //      // do captcha
         //      doCaptcha(parameters, result);
         //      await ExecuteMethodAsync(method, parameters);
         //   }
         //}

         return string.Empty;
      }

      private bool tryToHandleException(WebException ex) {
         string message;
         var innerException = ex.InnerException; 

         switch (ex.Status) {
            case WebExceptionStatus.ConnectFailure:
               message = $"{innerException?.Message ?? "Ошибка соединения с удаленным сервером."}\n\nCheck your proxy server if you use one.";
               break;
            default:
               return false;
         }

         throw new VkException(!string.IsNullOrEmpty(message) ? message : innerException?.Message ?? ex.Message, ex);
      }

      private Uri buildCompleteUri(string method, QueryParameters parameters) {
         var uriBuilder = new UriBuilder($"https://api.vk.com/method/{method}");

         var uriParameters = new NameValueCollection();

         if (parameters != null) {
            uriParameters.Add(parameters.Query);
         }

         uriParameters["access_token"] = _token.Token;
         uriParameters["v"] = VERSION;

         uriBuilder.Query = string.Join("&", uriParameters.AllKeys
            .Select(key => $"{key}={HttpUtility.UrlEncode(uriParameters[key])}"));

         return uriBuilder.Uri;
      }

      protected Error deserializeError(string jsonString) {
         return JsonConvert.DeserializeObject<ErrorResponse>(jsonString).Error;
      }

      protected virtual void OnCallPerformed(VkApiResponseInfo e) {
         CallPerformed?.Invoke(this, e);

         var key = e.Uri.ToString();
         _responseCache.TryAdd(key, e);

         _tooMuchRequestsOccurrences = 0;
         _timeoutRetry = 0;
      }
   }

   [UsedImplicitly]
   public class ErrorResponse {
      [JsonProperty(PropertyName = "error")]
      public Error Error { get; set; }
   }
}