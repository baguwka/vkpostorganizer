using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RateLimiter;

namespace vk.Models.Logger {
   [UsedImplicitly]
   public class JsonServerHistoryPublisher : IHistoryPublisher {
      private readonly Settings _settings;
      private readonly TimeLimiter _rateLimiter;
      private readonly HttpClient _httpClient;

      public JsonServerHistoryPublisher(Settings settings, HttpMessageHandler httpMessageHandler) {
         _settings = settings;
         _rateLimiter = TimeLimiter.GetFromMaxCountByInterval(5, TimeSpan.FromSeconds(1));
         _httpClient = new HttpClient(httpMessageHandler) {Timeout = TimeSpan.FromSeconds(4)};
      }

      //private async Task ensureDataBase(HistoryPost data) {
      //   var uriBuilder = new UriBuilder(_settings.History.Uri) {
      //      Path = $"posts{data.WallId}"
      //   };

      //   var uri = uriBuilder.Uri;

      //   var getResult = await _httpClient.GetAsync(uri);

      //   if (getResult.StatusCode == HttpStatusCode.NotFound) {
      //      var postj =
      //         new JObject(
      //            new JProperty($"posts{data.WallId}",
      //               new JArray()));

      //      uriBuilder = new UriBuilder(_settings.History.Uri) {
      //         Path = "db"
      //      };

      //      uri = uriBuilder.Uri;

      //      var content = new StringContent(postj.ToString(), Encoding.UTF8, "application/json");
      //      var result = await _rateLimiter.Perform(() => _httpClient.PostAsync(uri, content));
      //      var strres = await result.Content.ReadAsStringAsync();
      //      var some = strres.ToString();
      //   }
      //}

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
            if (result.IsSuccessStatusCode) {
               var contentResult = result.Content.ReadAsStringAsync();
            }
         }
         catch (OperationCanceledException) {
            //ignore
         }
      }
   }
}