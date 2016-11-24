using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   public class UsersGet : VkApiBase {
      public UsersGet(string token) : base(token) {
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

   [UsedImplicitly]
   public class User {
      [JsonProperty(PropertyName = "first_name")]
      public string FirstName { get; set; }

      [JsonProperty(PropertyName = "last_name")]
      public string LastName { get; set; }

      [JsonProperty(PropertyName = "photo_50")]
      public string UserPhotoUri { get; set; }
   }
}
