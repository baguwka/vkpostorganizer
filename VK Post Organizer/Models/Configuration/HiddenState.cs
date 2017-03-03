using System;
using System.Collections.Generic;

namespace vk.Models {
   [Serializable]
   public class HiddenState {
      public HiddenState() {
         LastSeenHistoryPostDatePerWall = new Dictionary<int, int>();
      }

      public Dictionary<int, int> LastSeenHistoryPostDatePerWall { get; private set; }

      public void Set(HiddenState other) {
         if (other == null) return;

         LastSeenHistoryPostDatePerWall = new Dictionary<int, int>(other.LastSeenHistoryPostDatePerWall);
      }

      public void SetLastSeenFor(int wallId, int date) {
         if (LastSeenHistoryPostDatePerWall.ContainsKey(wallId)) {
            LastSeenHistoryPostDatePerWall[wallId] = date;
         }
         else {
            LastSeenHistoryPostDatePerWall.Add(wallId, date);
         }
      }

      public int GetLastSeenFor(int wallId) {
         if (LastSeenHistoryPostDatePerWall.ContainsKey(wallId)) {
            return LastSeenHistoryPostDatePerWall[wallId];
         }
         return default(int);
      }
   }
}