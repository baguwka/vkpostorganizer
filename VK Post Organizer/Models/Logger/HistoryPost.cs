using System;
using System.Collections.Generic;

namespace vk.Models.Logger {
   [Serializable]
   public class HistoryPost {
      public int OwnerId { get;  set; }
      public int WallId { get;  set; }

      public int PublishDate { get;  set; }
      public int PostponedToDate { get;  set; }

      public List<string> AttachmentUrls { get;  set; }
   }
}