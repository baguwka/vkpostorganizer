using System.ComponentModel;

namespace vk.Models {
   public enum PostType {
      [Description("Only posts")]
      Post = 1,
      [Description("Only reposts")]
      Repost = 2,
      [Description("All")]
      Both = Post | Repost,
   }
}