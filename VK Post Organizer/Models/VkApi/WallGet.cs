using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class WallGet : VkApiBase {
      public WallGet([NotNull] AccessToken token, [NotNull] IWebClient webClient) : base(token, webClient) {
      }

      public async Task<WallGetResponse> GetAsync(int id, int count = 100, int offset = 0) {
         var parameters = makeAQuery(id, count, offset);
         var response = await ExecuteMethodAsync("wall.get", parameters);
         return JsonConvert.DeserializeObject<WallGetResponse>(response);
      }

      public async Task<WallGetResponse> GetAsync(int id, VkParameters sadsadasd) {
         var parameters = makeAQuery(id, 100, 0);
         parameters.AddParameters(sadsadasd);

         var response = await ExecuteMethodAsync("wall.get", parameters);
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