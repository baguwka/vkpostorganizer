using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class GroupsGetById : VkApiBase {
      public GroupsGetById(AccessToken token, IWebClient webClient) : base(token, webClient) {
      }

      public GroupsGetByIdResponse Get(int id, string fields = "") {
         var response = ExecuteMethod("groups.getById", $"group_id={id}&fields={fields}");
         checkForErrors(response);
         return JsonConvert.DeserializeObject<GroupsGetByIdResponse>(response);
      }
   }

   [UsedImplicitly]
   public class GroupsGetByIdResponse {
      [JsonProperty(PropertyName = "response")]
      public List<GroupGetByIdItem> Response { get; set; }
   }

   [UsedImplicitly]
   public class GroupGetByIdItem {
      [JsonProperty(PropertyName = "id")]
      public int ID { get; set; }

      [JsonProperty(PropertyName = "name")]
      public string Name { get; set; }

      [JsonProperty(PropertyName = "is_admin")]
      public int IsAdmin { get; set; }

      [JsonProperty(PropertyName = "is_member")]
      public int IsMember{ get; set; }

      [JsonProperty(PropertyName = "description")]
      public string Description { get; set; }

      [JsonProperty(PropertyName = "photo_50")]
      public string Photo50 { get; set; }

      [JsonProperty(PropertyName = "photo_100")]
      public string Photo100 { get; set; }

      [JsonProperty(PropertyName = "photo_200")]
      public string Photo200 { get; set; }
   }
}