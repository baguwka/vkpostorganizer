using System;
using System.Collections.Generic;
using System.Linq;

namespace vk.Utils {
   public static class EnumerableExtensions {
      public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
         foreach (var item in source) {
            action(item);
         }
      }

      public static bool None<TSource>(this IEnumerable<TSource> source) {
         return !source.Any();
      }

      public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
         return !source.Any(predicate);
      }
   }
}