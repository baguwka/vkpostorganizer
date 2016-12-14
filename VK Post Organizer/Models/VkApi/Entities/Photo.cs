using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Photo : BindableBase {
      private int _id;
      private string _photo75;
      private string _photo604;
      private string _photo1280;
      private int _date;
      private string _accessKey;

      [JsonProperty(PropertyName = "id")]
      public int ID {
         get { return _id; }
         set { SetProperty(ref _id, value); }
      }

      [JsonProperty(PropertyName = "photo_75")]
      public string Photo75 {
         get { return _photo75; }
         set { SetProperty(ref _photo75, value); }
      }

      [JsonProperty(PropertyName = "photo_604")]
      public string Photo604 {
         get { return _photo604; }
         set { SetProperty(ref _photo604, value); }
      }

      [JsonProperty(PropertyName = "photo_1280")]
      public string Photo1280 {
         get { return _photo1280; }
         set { SetProperty(ref _photo1280, value); }
      }

      [JsonProperty(PropertyName = "date")]
      public int Date {
         get { return _date; }
         set { SetProperty(ref _date, value); }
      }

      [JsonProperty(PropertyName = "access_key")]
      public string AccessKey {
         get { return _accessKey; }
         set { SetProperty(ref _accessKey, value); }
      }
   }
}