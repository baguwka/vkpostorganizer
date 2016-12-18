using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Document : BindableBase {
      private int _id;
      private int _size;
      private string _ext;
      private string _url;
      private int _date;
      private int _type;

      [JsonProperty(PropertyName = "id")]
      public int ID {
         get { return _id; }
         set { SetProperty(ref _id, value); }
      }

      [JsonProperty(PropertyName = "size")]
      public int Size {
         get { return _size; }
         set { SetProperty(ref _size, value); }
      }

      [JsonProperty(PropertyName = "ext")]
      public string Ext {
         get { return _ext; }
         set { SetProperty(ref _ext, value); }
      }

      [JsonProperty(PropertyName = "url")]
      public string Url {
         get { return _url; }
         set { SetProperty(ref _url, value); }
      }

      [JsonProperty(PropertyName = "date")]
      public int Date {
         get { return _date; }
         set { SetProperty(ref _date, value); }
      }

      [JsonProperty(PropertyName = "type")]
      public int Type {
         get { return _type; }
         set { SetProperty(ref _type, value); }
      }
   }

   public enum DocType {
      Text = 1,
      Archive = 2,
      Gif = 3,
      Image = 4,
      Audio = 5,
      Video = 6,
      Ebook = 7,
      Unknown = 8
   }
}