using JetBrains.Annotations;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Group : BindableBase, IWallHolder {
      private int _id;
      private string _photo200;
      private string _name;
      private string _photo50;
      private string _description;

      [JsonProperty(PropertyName = "id")]
      public int ID {
         get { return _id; }
         set { SetProperty(ref _id, value); }
      }

      [JsonProperty(PropertyName = "name")]
      public string Name {
         get { return _name; }
         set { SetProperty(ref _name, value); }
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
   }
}