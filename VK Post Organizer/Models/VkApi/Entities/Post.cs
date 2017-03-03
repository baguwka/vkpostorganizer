using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Prism.Mvvm;
using vk.Utils;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Post : BindableBase, IPost {
      private int _id;
      private int _fromId;
      private int _ownerId;
      private string _message;
      private int _date;
      private List<Attachment> _attachments;
      private string _dateString;
      private List<Post> _copyHistory;

      public Post() {
         Attachments = new List<Attachment>();
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
      public string Message {
         get { return _message; }
         set { SetProperty(ref _message, value); }
      }

      [JsonProperty(PropertyName = "date")]
      public int Date {
         get { return _date; }
         set {
            SetProperty(ref _date, value);
            DateString = UnixTimeConverter.ToDateTime(_date).ToString("dd.MM.yy HH:mm", CultureInfo.CurrentCulture);
         }
      }

      public DateTime DateTime => UnixTimeConverter.ToDateTime(_date);

      public string DateString {
         get { return _dateString; }
         private set { SetProperty(ref _dateString, value); }
      }

      [JsonProperty(PropertyName = "attachments")]
      public List<Attachment> Attachments {
         get { return _attachments; }
         set { SetProperty(ref _attachments, value); }
      }

      [JsonProperty(PropertyName = "copy_history", Required = Required.Default)]
      public List<Post> CopyHistory {
         get { return _copyHistory; }
         set { SetProperty(ref _copyHistory, value); }
      }
   }
}