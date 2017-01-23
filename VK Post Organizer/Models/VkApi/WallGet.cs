using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class WallGet : VkApiBase {
      public WallGet([NotNull] AccessToken token, [NotNull] IWebClient webClient) : base(token, webClient) {
      }

      public WallGetResponse Get(int id, int count = 100, int offset = 0) {
         id = -Math.Abs(id);

         var response = ExecuteMethod("wall.get", VkParameters.New()
                                                   .AddParameter("owner_id", id)
                                                   .AddParameter("filter", "postponed")
                                                   .AddParameter("offset", offset)
                                                   .AddParameter("count", count));

         return JsonConvert.DeserializeObject<WallGetResponse>(response);
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