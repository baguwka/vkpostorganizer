using JetBrains.Annotations;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class User : BindableBase, IWallHolder {
      private int _id;
      private string _photo50;
      private string _photo200;
      private string _firstName;
      private string _lastName;
      private string _description;

      [JsonProperty(PropertyName = "id")]
      public int ID {
         get { return _id; }
         set { SetProperty(ref _id, value); }
      }

      [JsonProperty(PropertyName = "first_name")]
      public string FirstName {
         get { return _firstName; }
         set { SetProperty(ref _firstName, value); }
      }

      [JsonProperty(PropertyName = "last_name")]
      public string LastName {
         get { return _lastName; }
         set { SetProperty(ref _lastName, value); }
      }

      [JsonProperty(PropertyName = "description")]
      public string Description {
         get { return _description; }
         set { SetProperty(ref _description, value); }
      }

      [JsonProperty(PropertyName = "photo_50")]
      public string Photo50 {
         get { return _photo50; }
         set { SetProperty(ref _photo50, value); }
      }

      [JsonProperty(PropertyName = "photo_200")]
      public string Photo200 {
         get { return _photo200; }
         set { SetProperty(ref _photo200, value); }
      }

      public string Name => $"{FirstName} {LastName}";
   }
}