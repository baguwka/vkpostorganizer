using System;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json;
using vk.Utils;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Post : BindableBase {
      private int _id;
      private string _text;
      private int _dateUnix;
      private SmartCollection<Attachment> _attachments;
      private string _dateString;
      private SmartCollection<Post> _copyHistory;

      public Post() {
         Attachments = new SmartCollection<Attachment>();
      }

      [JsonProperty(PropertyName = "id")]
      public int ID {
         get { return _id; }
         set { SetProperty(ref _id, value); }
      }

      [JsonProperty(PropertyName = "text")]
      public string Text {
         get { return _text; }
         set { SetProperty(ref _text, value); }
      }

      [JsonProperty(PropertyName = "date")]
      public int DateUnix {
         get { return _dateUnix; }
         set {
            SetProperty(ref _dateUnix, value);
            DateString = UnixTimeConverter.ToDateTime(_dateUnix).ToString("dd.MM.yy HH:mm", CultureInfo.CurrentCulture);
         }
      }

      public DateTime Date => UnixTimeConverter.ToDateTime(_dateUnix);

      public string DateString {
         get { return _dateString; }
         private set { SetProperty(ref _dateString, value); }
      }

      [JsonProperty(PropertyName = "attachments")]
      public SmartCollection<Attachment> Attachments {
         get { return _attachments; }
         set { SetProperty(ref _attachments, value); }
      }

      [JsonProperty(PropertyName = "copy_history", Required = Required.Default)]
      public SmartCollection<Post> CopyHistory {
         get { return _copyHistory; }
         set { SetProperty(ref _copyHistory, value); }
      }
   }
}