using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Attachment : BindableBase {
      private string _type;
      private Photo _photo;
      private Document _document;

      [JsonProperty(PropertyName = "type")]
      public string Type {
         get { return _type; }
         set { SetProperty(ref _type, value); }
      }

      [JsonProperty(PropertyName = "photo")]
      public Photo Photo {
         get { return _photo; }
         set { SetProperty(ref _photo, value); }
      }

      [JsonProperty(PropertyName = "doc")]
      public Document Document {
         get { return _document; }
         set { SetProperty(ref _document, value); }
      }
   }
}