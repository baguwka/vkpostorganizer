using System;

namespace vk.Models {
   [Flags]
   public enum PostType {
      Post = 1,
      Repost = 2,
      Missing = 4,
   }
}