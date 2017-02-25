using System;
using System.Collections.Generic;

namespace vk.Models.Logger {
   [Serializable]
   public class HistoryPost {
      public int OwnerId { get;  set; }
      public int WallId { get;  set; }

      public int PublishingDate { get;  set; }
      public int PostponedToDate { get;  set; }

      public string Message { get; set; }
      public List<string> AttachmentUrls { get;  set; }
   }
}