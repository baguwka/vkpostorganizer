using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using NLog;
using RateLimiter;
using vk.Models.VkApi;

namespace vk.Models.JsonServerApi {
   [UsedImplicitly]
   public class JsApi {
      private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

      private readonly HttpClient _httpClient;
      private readonly HistorySettings _historySettings;
      private readonly TimeLimiter _rateLimiter;

      private int _timeoutRetry;

      public JsApi(HttpMessageHandler httpClientHandler, HistorySettings historySettings) {
         _historySettings = historySettings;
         _httpClient = new HttpClient(httpClientHandler) { Timeout = TimeSpan.FromSeconds(4) };
         _rateLimiter = TimeLimiter.GetFromMaxCountByInterval(3, TimeSpan.FromSeconds(1.00f));
      }

      public async Task<string> CallAsync(string path, QueryParameters query, CancellationToken ct) {
         var uri = buildUri(path, query);
         logger.Debug($"Запрос к json серверу. Итоговый url - {uri}");
         try {
            var response = await _rateLimiter.Perform(() => _httpClient.GetAsync(uri, ct), ct).ConfigureAwait(false);
            
            if (!response.IsSuccessStatusCode) {
               logger.Error($"Ошибка при обрещении к Json Server. Http Status Code - {response.StatusCode}, url - {uri}");
               throw new JsonServerException($"Ошибка при обращении к Json Server. Http status code - {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            checkForErrors(result);

            return result;
         }

         // Httpclient timeout
         catch (TaskCanceledException ex) {
            _timeoutRetry++;
            if (!ct.IsCancellationRequested) {
               logger.Error($"Превышено время ожидания, url - {uri}");
               if (_timeoutRetry > 2) {
                  //var error = "";
                  //if (_httpClientHandler.UseProxy) {
                  //   error = "Проверьте настройки прокси сервера и перезапустите приложение.";
                  //}
                  logger.Error(ex, $"Превышено время ожидания, соединение не удалось, url - {uri}");
                  throw new JsonServerException(
                     $"Соеденение не удалось.\nПроверьте настройки прокси сервера и перезапустите приложение.\n{ex.Message}",
                     ex);
               }

               return await CallAsync(path, query, ct).ConfigureAwait(false);
            }

            //it's not timeout
            logger.Debug($"Операция отменена для url - {uri}");
            throw;
         }
      }

      private void checkForErrors(string response) {
         if (string.IsNullOrEmpty(response) || response.Length < 5) {
            throw new JsonServerException($"Не получен ответ от сервера");
         }
      }

      private Uri buildUri(string path, QueryParameters parameters) {
         var uriBuilder = new UriBuilder(_historySettings.Uri) {Path = path};
         var uriParameters = new NameValueCollection();

         if (parameters != null) {
            uriParameters.Add(parameters.Query);
         }

         uriBuilder.Query = string.Join("&", uriParameters.AllKeys
            .Select(key => $"{key}={HttpUtility.UrlEncode(uriParameters[key])}"));

         return uriBuilder.Uri;
      }
   }
}