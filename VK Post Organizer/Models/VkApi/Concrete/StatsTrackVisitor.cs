using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class StatsTrackVisitor {
      private readonly VkApi _api;

      public StatsTrackVisitor(VkApi api) {
         this._api = api;
      }

      public async Task<int> TrackAsync(CancellationToken ct) {
         var response = await _api.ExecuteMethodAsync("stats.trackVisitor", QueryParameters.No(), ct).ConfigureAwait(false);
         return JsonConvert.DeserializeObject<StatsTrackVisitorResponse>(response).Response;
      }

      public async Task<int> TrackAsync() {
         return await TrackAsync(CancellationToken.None).ConfigureAwait(false);
      }
   }

   [UsedImplicitly]
   public class StatsTrackVisitorResponse {
      [JsonProperty(PropertyName = "response")]
      public int Response { get; set; }
   }
}