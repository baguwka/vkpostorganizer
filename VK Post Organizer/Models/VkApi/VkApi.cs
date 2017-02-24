using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
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
   public class VkApiResponse {
      public VkApiResponse(string response, DateTimeOffset storedAt) {
         Response = response;
         StoredAt = storedAt;
      }

      public string Response { get; private set; }
      public DateTimeOffset StoredAt { get; private set; }
   }

   public class VkCall {
      public Action<string> Callback { get; set; }
      public Task<string> Task { get; set; }
   }

   [UsedImplicitly]
   public class VkApi {
      public const string VERSION = "5.62";

      private readonly HttpClient _httpClient;
      private readonly HttpClientHandler _httpClientHandler;
      private readonly AccessToken _token;
      private readonly ConcurrentDictionary<string, VkApiResponse> _responseCache;

      private readonly TimeLimiter _rateLimiter;
      private int _tooMuchRequestsOccurrences;
      private int _timeoutRetry;


      public VkApi(AccessToken token, HttpClientHandler httpClientHandler) {
         _token = token;
         _httpClientHandler = httpClientHandler;
         _httpClient = new HttpClient(httpClientHandler) {Timeout = TimeSpan.FromSeconds(4)};
         _responseCache = new ConcurrentDictionary<string, VkApiResponse>();

         _rateLimiter = TimeLimiter.GetFromMaxCountByInterval(6, TimeSpan.FromSeconds(1));
      }

      private void checkForErrors(string response) {
         if (string.IsNullOrEmpty(response) || response.Length < 5) {
            throw new VkException($"Не получен ответ от сервера");
         }

         if (response.Substring(2, 5) == "error") {
            var error = deserializeError(response);
            throw new VkException($"Код ошибки: {error.ErrorCode}\n{error.ErrorMessage}", error.ErrorCode);
         }
      }

      public async Task<string> ExecuteMethodAsync(string method, [NotNull] VkParameters query, CancellationToken ct) {
         if (query == null) {
            throw new ArgumentNullException(nameof(query));
         }

         var uri = buildFinalUri(method, query);
         if (_responseCache.ContainsKey(uri.ToString())) {
            VkApiResponse response;
            var result = _responseCache.TryGetValue(uri.ToString(), out response);
            if (result) {
               var timeSpan = DateTimeOffset.Now - response.StoredAt;
               if (timeSpan < TimeSpan.FromSeconds(5)) {
                  return response.Response;
               }
               else {
                  _responseCache.TryRemove(uri.ToString(), out response);
               }
            }
         }

         return await _rateLimiter.Perform(() => callUriAsync(uri, ct), ct).ConfigureAwait(false);
      }

      private async Task<string> callUriAsync(Uri uri, CancellationToken ct) {
         try {
            var response = await _httpClient.GetAsync(uri, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            checkForErrors(result);

            _responseCache.TryAdd(uri.ToString(), new VkApiResponse(result, DateTimeOffset.Now));

            _tooMuchRequestsOccurrences = 0;
            _timeoutRetry = 0;

            return result;
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
               await Task.Delay(TimeSpan.FromSeconds(1f), ct).ConfigureAwait(false);
               if (!ct.IsCancellationRequested) {
                  return await callUriAsync(uri, ct);
               }
            }

            throw;
         }
         // Httpclient timeout
         catch (TaskCanceledException ex) {
            if (_timeoutRetry > 2) {
               var error = "";
               if (_httpClientHandler.UseProxy) {
                  error = "Проверьте настройки прокси сервера и перезапустите приложение.";
               }
               throw new VkException($"Соеденение не удалось.\n{error}", ex);
            }

            _timeoutRetry++;
            if (!ct.IsCancellationRequested) {
               return await callUriAsync(uri, ct).ConfigureAwait(false);
            }
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

      private Uri buildFinalUri(string method, VkParameters parameters) {
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
   }

   [UsedImplicitly]
   public class ErrorResponse {
      [JsonProperty(PropertyName = "error")]
      public Error Error { get; set; }
   }
}