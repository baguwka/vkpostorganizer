using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
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
      public GroupGetResponseResponse Response { get; set; }
   }

   [UsedImplicitly]
   public class GroupGetResponseResponse {
      [JsonProperty(PropertyName = "count")]
      public int Count { get; set; }
      [JsonProperty(PropertyName = "items")]
      public List<Group> Groups { get; set; }
   }

   [UsedImplicitly]
   public class Group {
      [JsonProperty(PropertyName = "gid")]
      public int ID { get; set; }

      [JsonProperty(PropertyName = "name")]
      public string Name { get; set; }

      [JsonProperty(PropertyName = "photo_50")]
      public string Photo50 { get; set; }

      [JsonProperty(PropertyName = "photo_200")]
      public string Photo200 { get; set; }
   }
}