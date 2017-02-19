using System;
using System.Collections.ObjectModel;
using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Prism.Mvvm;
using vk.Utils;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Post : BindableBase {
      private int _id;
      private int _fromId;
      private int _ownerId;
      private string _text;
      private int _dateUnix;
      private ObservableCollection<Attachment> _attachments;
      private string _dateString;
      private ObservableCollection<Post> _copyHistory;

      public Post() {
         Attachments = new ObservableCollection<Attachment>();
      }
      
      [JsonProperty(PropertyName = "id")]
      public int ID {
         get { return _id; }
         set { SetProperty(ref _id, value); }
      }

      [JsonProperty(PropertyName = "from_id")]
      public int FromId {
         get { return _fromId; }
         set { SetProperty(ref _fromId, value); }
      }

      [JsonProperty(PropertyName = "owner_id")]
      public int OwnerId {
         get { return _ownerId; }
         set { SetProperty(ref _ownerId, value); }
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
      public ObservableCollection<Attachment> Attachments {
         get { return _attachments; }
         set { SetProperty(ref _attachments, value); }
      }

      [JsonProperty(PropertyName = "copy_history", Required = Required.Default)]
      public ObservableCollection<Post> CopyHistory {
         get { return _copyHistory; }
         set { SetProperty(ref _copyHistory, value); }
      }
   }
}