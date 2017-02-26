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

      public async Task<WallGetResponse> GetAsync(int id, int count = 100, int offset = 0, bool ignoraCache = false) {
         var parameters = makeAQuery(id, count, offset);
         return await GetAsync(parameters, ignoraCache).ConfigureAwait(false);
      }

      public async Task<WallGetResponse> GetAsync(VkParameters parameters, bool ignoraCache = false) {
         return await GetAsync(parameters, CancellationToken.None, ignoraCache);
      }

      public async Task<WallGetResponse> GetAsync(VkParameters parameters, CancellationToken ct, bool ignoraCache = false) {
         var response = ignoraCache 
            ? await _api.ExecuteMethodIgnoreCacheAsync("wall.get", parameters, ct).ConfigureAwait(false)
            : await _api.ExecuteMethodAsync("wall.get", parameters, ct).ConfigureAwait(false);

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
      public WallGetCollection Content { get; set; }
   }

   [UsedImplicitly]
   public class WallGetCollection {
      [JsonProperty(PropertyName = "count")]
      public int Count { get; set; }

      [JsonProperty(PropertyName = "items")]
      public List<Post> Wall { get; set; }
   }
}