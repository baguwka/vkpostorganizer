using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace vk.Models.Logger {
   public class JsonServerPublishLogger : IPublishLogger {
      private readonly Settings _settings;

      public JsonServerPublishLogger(Settings settings) {
         _settings = settings;
      }

      public async Task LogAsync(HistoryPost data) {
         var uri = new UriBuilder(_settings.History.Uri.Authority).Uri;
         var client = new RestClient(uri);
         var request = new RestRequest("posts", Method.POST);
         var serialized = JsonConvert.SerializeObject(data);

         request.AddParameter("info", serialized);

         client.ExecuteAsync(request, restResponse => {
            var some = restResponse.Content;
         });
      }

      public void Log(HistoryPost data) {
         throw new System.NotImplementedException();
      }
   }
}