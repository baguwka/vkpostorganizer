using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class GroupsGet : VkApiBase {
      public GroupsGet(AccessToken token, IWebClient webClient) : base(token, webClient) {
      }

      public GroupsGetResponse Get() {
         var response = ExecuteMethod("groups.get", "extended=1&filter=editor");
         return JsonConvert.DeserializeObject<GroupsGetResponse>(response);
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