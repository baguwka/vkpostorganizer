using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class GroupsGet : VkApiBase {
      public GroupsGet(AccessToken token, IWebClient webClient) : base(token, webClient) {
      }

      [Obsolete]
      public GroupsGetResponse Get() {
         var query = buildAQuery();
         var response = ExecuteMethod("groups.get", query);
         return JsonConvert.DeserializeObject<GroupsGetResponse>(response);
      }

      public async Task<GroupsGetResponse> GetAsync() {
         var query = buildAQuery();
         var response = await ExecuteMethodAsync("groups.get", query);
         return JsonConvert.DeserializeObject<GroupsGetResponse>(response);
      }

      private static VkParameters buildAQuery() {
         return VkParameters.New()
            .AddParameter("extended", 1)
            .AddParameter("filter", "editor")
            .AddParameter("fields", "description");
      }
   }

   [UsedImplicitly]
   public class GroupsGetResponse {
      [JsonProperty(PropertyName = "response")]
      public GroupGetCollection Collection { get; set; }
   }

   [UsedImplicitly]
   public class GroupGetCollection {
      [JsonProperty(PropertyName = "count")]
      public int Count { get; set; }
      [JsonProperty(PropertyName = "items")]
      public List<Group> Groups { get; set; }
   }
}