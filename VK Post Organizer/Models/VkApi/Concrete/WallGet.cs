using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class WallGet {
      private readonly VkApi _api;

      public WallGet(VkApi api) {
         this._api = api;
      }

      public async Task<WallGetResponse> GetAsync(int id, int count = 100, int offset = 0) {
         var parameters = makeAQuery(id, count, offset);
         return await GetAsync(parameters).ConfigureAwait(false);
      }

      public async Task<WallGetResponse> GetAsync(int id, VkParameters parameters, CancellationToken ct) {
         parameters.AddParameter("owner_id", -Math.Abs(id));
         return await GetAsync(parameters, ct);
      }

      public async Task<WallGetResponse> GetAsync(VkParameters parameters) {
         return await GetAsync(parameters, CancellationToken.None);
      }

      public async Task<WallGetResponse> GetAsync(VkParameters parameters, CancellationToken ct) {
         var response = await _api.ExecuteMethodAsync("wall.get", parameters, ct).ConfigureAwait(false);
         return JsonConvert.DeserializeObject<WallGetResponse>(response);
      }

      private static VkParameters makeAQuery(int id, int count, int offset) {
         id = -Math.Abs(id);
         var parameters = VkParameters.New()
            .AddParameter("owner_id", id)
            .AddParameter("filter", "postponed")
            .AddParameter("offset", offset)
            .AddParameter("count", count);
         return parameters;
      }
   }

   [UsedImplicitly]
   public class WallGetResponse {
      [JsonProperty(PropertyName = "response")]
      public WallGetCollection Response { get; set; }
   }

   [UsedImplicitly]
   public class WallGetCollection {
      [JsonProperty(PropertyName = "count")]
      public int Count { get; set; }

      [JsonProperty(PropertyName = "items")]
      public List<Post> Wall { get; set; }
   }
}