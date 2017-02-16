using JetBrains.Annotations;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Photo : BindableBase {
      private int _id;
      private string _photo75;
      private string _photo130;
      private string _photo604;
      private string _photo1280;
      private string _photo2560;
      private int _date;
      private string _text;
      private int _ownerId;

      [JsonProperty(PropertyName = "id")]
      public int Id {
         get { return _id; }
         set { SetProperty(ref _id, value); }
      }

      [JsonProperty(PropertyName = "owner_id")]
      public int OwnerId {
         get { return _ownerId; }
         set { SetProperty(ref _ownerId, value); }
      }

      [JsonProperty(PropertyName = "photo_75")]
      public string Photo75 {
         get { return _photo75; }
         set { SetProperty(ref _photo75, value); }
      }

      [JsonProperty(PropertyName = "photo_130")]
      public string Photo130 {
         get { return _photo130; }
         set { SetProperty(ref _photo130, value); }
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

      [JsonProperty(PropertyName = "photo_2560")]
      public string Photo2560 {
         get { return _photo2560; }
         set { SetProperty(ref _photo2560, value); }
      }

      [JsonProperty(PropertyName = "date")]
      public int Date {
         get { return _date; }
         set { SetProperty(ref _date, value); }
      }

      [JsonProperty(PropertyName = "text")]
      public string Text {
         get { return _text; }
         set { SetProperty(ref _text, value); }
      }

      public string GetLargest() {
         if (!string.IsNullOrEmpty(Photo2560)) {
            return Photo2560;
         }

         if (!string.IsNullOrEmpty(Photo1280)) {
            return Photo1280;
         }

         if (!string.IsNullOrEmpty(Photo604)) {
            return Photo604;
         }

         if (!string.IsNullOrEmpty(Photo75)) {
            return Photo75;
         }

         return "";
      }
   }
}