using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class StatsTrackVisitor {
      private readonly VkApiBase _api;

      public StatsTrackVisitor(VkApiBase api) {
         this._api = api;
      }

      public async Task<int> TrackAsync() {
         var response = await _api.ExecuteMethodAsync("stats.trackVisitor", VkParameters.No());
         return JsonConvert.DeserializeObject<StatsTrackVisitorResponse>(response).Response;
      }
   }

   [UsedImplicitly]
   public class StatsTrackVisitorResponse {
      [JsonProperty(PropertyName = "response")]
      public int Response { get; set; }
   }
}