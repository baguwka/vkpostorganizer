using System;

namespace vk.Utils {
   public static class SizeHelper {
      static readonly string[] sizeSuffixes =
      { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

      public static string Suffix(long value) {
         if (value < 0) { return "-" + Suffix(-value); }
         if (value == 0) { return "0.0 bytes"; }

         var mag = (int)Math.Log(value, 1024);
         var adjustedSize = (decimal)value / (1L << (mag * 10));

         return $"{adjustedSize:n1} {sizeSuffixes[mag]}";
      }
   }
}