using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class StatsTrackVisitor : VkApiBase {
      public StatsTrackVisitor([NotNull] AccessToken token, [NotNull] IWebClient webClient) : base(token, webClient) {
      }

      [Obsolete]
      public int Track() {
         var response = ExecuteMethod("stats.trackVisitor", VkParameters.No());
         return JsonConvert.DeserializeObject<StatsTrackVisitorResponse>(response).Response;
      }

      public async Task<int> TrackAsync() {
         var response = await ExecuteMethodAsync("stats.trackVisitor", VkParameters.No());
         return JsonConvert.DeserializeObject<StatsTrackVisitorResponse>(response).Response;
      }
   }

   [UsedImplicitly]
   public class StatsTrackVisitorResponse {
      [JsonProperty(PropertyName = "response")]
      public int Response { get; set; }
   }
}