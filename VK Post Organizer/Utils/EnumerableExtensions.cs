using System;
using System.Collections.Generic;

namespace vk.Utils {
   public static class EnumerableExtensions {
      public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
         foreach (var item in source) {
            action(item);
         }
      }
   }
}