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
using NLog;
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
      private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

      public const string VERSION = "5.64";

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
            var msg = $"Не получен ответ от сервера.";
            logger.Error(msg);
            throw new VkException(msg);
         }

         if (response.Substring(2, 5) == "error") {
            var error = deserializeError(response);
            logger.Error($"Vk error code: {error.ErrorCode}. {error.ErrorMessage}");
            throw new VkException($"Код ошибки: {error.ErrorCode}\n{error.ErrorMessage}", error.ErrorCode);
         }
      }
      
      public async Task<string> ExecuteMethodAsync(string method, [NotNull] QueryParameters query, CancellationToken ct) {
         if (query == null) {
            throw new ArgumentNullException(nameof(query));
         }


         var uri = buildCompleteUri(method, query);

         logger.Debug($"Запрос на выполнение метода Vk api с проверкой на наличие в кэше и ограничением по множеству запросов - {method}. Итоговый url для перехода - {uri}");

         var key = uri.ToString();

         //защита от множества запросов за короткое время, что бы они возможно получили данные из кэша
         if (_requestLog.ContainsKey(key)) {
            DateTimeOffset lastLog;
            if (_requestLog.TryGetValue(key, out lastLog)) {
               var timeSpan = DateTimeOffset.Now - lastLog;
               if (timeSpan < TimeSpan.FromSeconds(1)) {
                  logger.Debug($"Защита от множества запросов за короткое время обнаружила что подобный запрос выполнялся совсем недавно." +
                               $"Запрос будет задержан на некоторое время.");
                  logger.Debug($"Задержка для запроса по url {uri} на 1 секунду.");
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
                  logger.Debug($"Система кеширования обнаружила, что подобный запрос выполнялся совсем недавно, а значит данные будут получены из кэша.");
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

         logger.Debug($"Запрос на выполнение метода Vk api без каких либо проверок на наличие в кэше - {method}. Итоговый url для перехода - {uri}");

         return await _rateLimiter.Perform(() => callAsync(uri, ct), ct).ConfigureAwait(false);
      }

      private async Task<string> callAsync(Uri uri, CancellationToken ct) {
         try {

            logger.Debug($"Выполнение запроса к серверу по url - {uri}");

            var response = await _httpClient.GetAsync(uri, ct).ConfigureAwait(false);

            logger.Trace($"Ответ получен для url {uri}");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            checkForErrors(result);

            var vkResponse = new VkApiResponseInfo(result, DateTimeOffset.Now) {
               Uri = uri
            };

            logger.Trace($"Ответ успешно получен и сериализован для url {uri}");
            OnCallPerformed(vkResponse);

            return result;
         }
         catch (HttpRequestException ex) {
            logger.Error(ex, $"Соединение по url {uri} не удалось. Возможны проблемы с прокси сервером, либо сервер не отвечает.");
            throw new VkException($"Соеденение не удалось.\nПроверьте настройки прокси сервера и перезапустите приложение.\n{ex.Message}", ex);
         }
         catch (WebException ex) {
            logger.Error(ex, "Получено исключение WebException. Trying to handle it.");
            if (tryToHandleException(ex) == false) {
               logger.Error(ex, "Attempt to handle exception fails. Rethrowing");
               throw;
            }
         }
         catch (VkException ex) {
            logger.Debug(ex, "Поймано исключение VkException. Возможно превышено ограничение на количество запросов в секунду, капча или что то еще.");
            if (_tooMuchRequestsOccurrences > 2) {
               logger.Error(ex, "Количество попыток на повтор запроса иссекли, слишком много запросов в секунду.");
               throw;
            }

            _tooMuchRequestsOccurrences++;
            //too much requests per second
            if (ex.ErrorCode == 6) {
               logger.Debug(ex, $"Слишком много запросов в секунду. После небольшой задержки запрос будет повторен. Вызванный url - {uri}");
               await Task.Delay(TimeSpan.FromSeconds(1f), ct);
               if (!ct.IsCancellationRequested) {
                  return await callAsync(uri, ct);
               }
            }

            throw;
         }
         // Httpclient timeout
         catch (TaskCanceledException ex) {
            logger.Error(ex, $"Превышен таймаут ожидания ответа, либо операция была отменена. Вызванный url - {uri}");
            if (ct.IsCancellationRequested) {
               throw;
            }

            if (_timeoutRetry > 1) {
               logger.Error(ex, $"Соединение не удалось. Вызванный url - {uri}");
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
         logger.Error(ex, $"Attempt to handle exception {ex}");
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