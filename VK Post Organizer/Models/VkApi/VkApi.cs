using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class VkApi {
      public const string VERSION = "5.62";

      private readonly HttpClient _httpClient;
      private readonly HttpClientHandler _httpClientHandler;
      private readonly AccessToken _token;

      private int _failedAttempts;
      private int _timeoutRetry;
      private float _requestsPerSecond;
      private float _minInterval;

      public DateTimeOffset? LastExecuteTime { get; private set; }

      public TimeSpan? LastExecuteTimeSpan {
         get {
            if (LastExecuteTime.HasValue) {
               return DateTimeOffset.Now - LastExecuteTime.Value;
            }
            return null;
         }
      }

      public float RequestsPerSecond {
         get { return _requestsPerSecond; }
         set {
            if (value < 0) {
               throw new ArgumentException(@"Value must be positive", $@"RequestsPerSecond");
            }
            _requestsPerSecond = value;
            if (_requestsPerSecond > 0) {
               _minInterval = (int)(1000 / _requestsPerSecond) + 1;
            }
         }
      }

      public VkApi(AccessToken token, HttpClientHandler httpClientHandler) {
         _token = token;
         _httpClientHandler = httpClientHandler;
         _httpClient = new HttpClient(httpClientHandler) {Timeout = TimeSpan.FromSeconds(4)};

         RequestsPerSecond = 3;
         LastExecuteTime = DateTimeOffset.Now;
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

      public async Task<string> ExecuteMethodAsync(string method, [NotNull] VkParameters query) {
         if (query == null) {
            throw new ArgumentNullException(nameof(query));
         }

         if (RequestsPerSecond > 0 && LastExecuteTime.HasValue) {
            var span = LastExecuteTimeSpan?.TotalMilliseconds;
            if (span < _minInterval) {
               var timeout = (int)_minInterval - (int)span;
               await Task.Delay(timeout).ConfigureAwait(false);
            }

            return await executeMethodAsync(method, query).ConfigureAwait(false);
         }

         return string.Empty;
      }

      private async Task<string> executeMethodAsync(string method, [NotNull] VkParameters query) {
         var finalUri = buildFinalUri(method, query);
         try {
            LastExecuteTime = DateTimeOffset.Now;
            var response = await _httpClient.GetAsync(finalUri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            checkForErrors(result);
            _failedAttempts = 0;
            _timeoutRetry = 0;
            return result;
         }
         catch (WebException ex) {
            if (tryToHandleException(ex) == false) {
               throw;
            }
         }
         catch (VkException ex) {
            if (_failedAttempts > 3) {
               throw;
            }

            _failedAttempts++;
            //too much requests per second
            if (ex.ErrorCode == 6) {
               await Task.Delay(TimeSpan.FromSeconds(1f));
               return await ExecuteMethodAsync(method, query);
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
            return await ExecuteMethodAsync(method, query).ConfigureAwait(false);
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