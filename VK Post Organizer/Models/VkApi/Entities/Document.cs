using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Prism.Mvvm;

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

      [JsonProperty(PropertyName = "preview")]
      public DocumentPreview Preview { get; set; }
   }

   [UsedImplicitly]
   public class DocumentPreview {
      [JsonProperty(PropertyName = "photo")]
      public  DocumentPreviewPhoto Photo { get; set; }
   }

   [UsedImplicitly]
   public class DocumentPreviewPhoto {
      [JsonProperty(PropertyName = "sizes")]
      public List<DocumentPreviewPhotoSizes> Sizes { get; set; }
   }

   [UsedImplicitly]
   public class DocumentPreviewPhotoSizes {
      [JsonProperty(PropertyName = "src")]
      public string Source { get; set; }

      [JsonProperty(PropertyName = "width")]
      public int Width { get; set; }

      [JsonProperty(PropertyName = "height")]
      public int Height { get; set; }

      [JsonProperty(PropertyName = "type")]
      public string Type { get; set; }
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