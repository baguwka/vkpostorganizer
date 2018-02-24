using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using NLog;
using RateLimiter;

namespace vk.Models.History {
   [UsedImplicitly]
   public class JsonServerHistoryPublisher : IHistoryPublisher {
      private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

      private readonly Settings _settings;
      private readonly TimeLimiter _rateLimiter;
      private readonly HttpClient _httpClient;

      public JsonServerHistoryPublisher(Settings settings, HttpMessageHandler httpMessageHandler) {
         _settings = settings;
         _rateLimiter = TimeLimiter.GetFromMaxCountByInterval(5, TimeSpan.FromSeconds(1));
         _httpClient = new HttpClient(httpMessageHandler) {Timeout = TimeSpan.FromSeconds(4)};
      }

      public async Task LogAsync(HistoryPost data) {
         if (!_settings.History.Use || !Utils.UrlHelper.IsUriIsValid(_settings.History.Uri)) {
            return;
         }

         var uriBuilder = new UriBuilder(_settings.History.Uri) {
            Path = "posts"
         };

         var uri = uriBuilder.Uri;

         //await ensureDataBase(data);

         var serialized = JsonConvert.SerializeObject(data);
         var content = new StringContent(serialized, Encoding.UTF8, "application/json");

         try {
            var result = await _rateLimiter.Perform(() => _httpClient.PostAsync(uri, content));
            Debug.WriteLine($"--- HISTORY PUBLISHED AT {DateTimeOffset.Now.Ticks}");
            if (result.IsSuccessStatusCode) {
               var contentResult = result.Content.ReadAsStringAsync();
            }
         }
         catch (OperationCanceledException) {
            logger.Debug($"Операция добавления истории для поста {data.WallId}_{data.PostId} отменена.");
         }
      }
   }
}