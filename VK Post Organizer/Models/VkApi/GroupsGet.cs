using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   public class GroupsGet : VkApiBase {
      public GroupsGet(string token) : base(token) {
      }
   }

   [UsedImplicitly]
   public class GroupsGetResponse {
      [JsonProperty(PropertyName = "count")]
      public int Count { get; set; }

      [JsonProperty(PropertyName = "items")]
      public List<Group> Groups { get; set; }
   }

   [UsedImplicitly]
   public class Group {
      [JsonProperty(PropertyName = "id")]
      public int ID { get; set; }

      [JsonProperty(PropertyName = "name")]
      public string Name { get; set; }

      [JsonProperty(PropertyName = "photo_50")]
      public string Photo50 { get; set; }

      [JsonProperty(PropertyName = "photo_200")]
      public string Photo200 { get; set; }
   }
}