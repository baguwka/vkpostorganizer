using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Prism.Mvvm;
using vk.Utils;
using vk.ViewModels;

namespace vk.Models.History {
   [Serializable]
   public class HistoryPost : BindableBase {
      private string _postponedDateString;
      private string _publishingDateString;

      private int _ownerId;
      private int _wallId;
      private bool _isRepost;
      private int _publishingDateUnix;
      private int _postponedDateUnix;
      private string _message;
      private IEnumerable<string> _attachments;

      [JsonProperty(PropertyName = "owner_id")]
      public int OwnerId {
         get { return _ownerId; }
         set { SetProperty(ref _ownerId, value); }
      }

      [JsonProperty(PropertyName = "wall_id")]
      public int WallId {
         get { return _wallId; }
         set { SetProperty(ref _wallId, value); }
      }

      [JsonProperty(PropertyName = "is_repost")]
      public bool IsRepost {
         get { return _isRepost; }
         set { SetProperty(ref _isRepost, value); }
      }

      [JsonProperty(PropertyName = "publishing_date")]
      public int PublishingDateUnix {
         get { return _publishingDateUnix; }
         set { SetProperty(ref _publishingDateUnix, value);
            PublishingDateString = UnixTimeConverter.ToDateTime(_publishingDateUnix).ToString("dd.MM.yy HH:mm", CultureInfo.CurrentCulture);
         }
      }

      [JsonProperty(PropertyName = "postponed_date")]
      public int PostponedDateUnix {
         get { return _postponedDateUnix; }
         set { SetProperty(ref _postponedDateUnix, value);
            PostponedDateString = UnixTimeConverter.ToDateTime(_postponedDateUnix).ToString("dd.MM.yy HH:mm", CultureInfo.CurrentCulture);
         }
      }

      [JsonProperty(PropertyName = "message")]
      public string Message {
         get { return _message; }
         set { SetProperty(ref _message, value); }
      }

      [JsonProperty(PropertyName = "attachments")]
      public IEnumerable<string> Attachments {
         get { return _attachments; }
         set { SetProperty(ref _attachments, value); }
      }

      /// <summary>
      /// Дата, когда пост был опубликован из приложения
      /// </summary>
      public DateTime PublishingDate => UnixTimeConverter.ToDateTime(_publishingDateUnix);

      /// <summary>
      /// Дата, на которую был отложен пост
      /// </summary>
      public DateTime PostponedDate => UnixTimeConverter.ToDateTime(_postponedDateUnix);

      public string PostponedDateString {
         get { return _postponedDateString; }
         private set { SetProperty(ref _postponedDateString, value); }
      }

      public string PublishingDateString {
         get { return _publishingDateString; }
         private set { SetProperty(ref _publishingDateString, value); }
      }
   }
}