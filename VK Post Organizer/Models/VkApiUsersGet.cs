using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models {
   public class VkApiUsersGet : VkApiBase {
      public VkApiUsersGet(string token) : base(token) {
      }

      public VkApiUsersGetResponse Get() {
         var response = ExecuteMethod("users.get", "fields=first_name,last_name,photo_50");
         MessageBox.Show(response);
         return JsonConvert.DeserializeObject<VkApiUsersGetResponse>(response);
      }
   }

   [UsedImplicitly]
   public class VkApiUsersGetResponse {
      [JsonProperty(PropertyName = "response")]
      public List<VkApiUser> Users { get; set; }
   }

   [UsedImplicitly]
   public class VkApiUser {
      [JsonProperty(PropertyName = "first_name")]
      public string FirstName { get; set; }

      [JsonProperty(PropertyName = "last_name")]
      public string LastName { get; set; }

      [JsonProperty(PropertyName = "photo_50")]
      public string UserPhotoUri { get; set; }
   }
}
