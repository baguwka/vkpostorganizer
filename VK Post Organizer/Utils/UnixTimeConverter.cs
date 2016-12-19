using System;

namespace vk.Utils {
   public static class UnixTimeConverter {
      public static DateTime ToDateTime(int time) {
         var info = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
         return new DateTime(1970, 1, 1, info.Hours, 0, 0, 0, DateTimeKind.Utc).AddSeconds(time);

      }

      public static int ToUnix(DateTime time) {
         var info = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
         return (int)(time.Subtract(new DateTime(1970, 1, 1, info.Hours, 0, 0))).TotalSeconds;
      }
   }
}
