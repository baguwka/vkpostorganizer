using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class UsersGet : VkApiBase {
      public UsersGet(AccessToken token, IWebClient webClient) : base(token, webClient) {
      }

      public UsersGetResponse Get() {
         var response = ExecuteMethod("users.get", "fields=first_name,last_name,photo_50");
         return JsonConvert.DeserializeObject<UsersGetResponse>(response);
      }
   }

   [UsedImplicitly]
   public class UsersGetResponse {
      [JsonProperty(PropertyName = "response")]
      public List<User> Users { get; set; }
   }
}
