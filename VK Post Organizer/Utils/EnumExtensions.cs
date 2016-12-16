using System;
using vk.Models;
using vk.Models.Filter;

namespace vk.Utils {
   public static class EnumExtensions {
      public static IPostFilter GetFilter(this PostType type) {
         switch (type) {
            case PostType.Post:
               return new PostsOnlyFilter();
            case PostType.Repost:
               return new RepostsOnlyFilter();
            case PostType.Both:
               return new NoPostFilter();
            default:
               throw new ArgumentOutOfRangeException(nameof(type), type, null);
         }
      }
   }
}