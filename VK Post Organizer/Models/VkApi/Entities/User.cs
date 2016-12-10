using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class User : BindableBase {
      [JsonProperty(PropertyName = "first_name")]
      public string FirstName { get; set; }

      [JsonProperty(PropertyName = "last_name")]
      public string LastName { get; set; }

      [JsonProperty(PropertyName = "photo_50")]
      public string UserPhotoUri { get; set; }
   }
}