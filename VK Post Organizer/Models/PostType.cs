using System.ComponentModel;

namespace vk.Models {
   public enum PostType {
      [Description("Only posts")]
      Post = 1,
      [Description("Only reposts")]
      Repost = 2,
      [Description("Missing")]
      Missing = 4,
      [Description("Posts and reposts")]
      PostOrRepost = Post | Repost,
      [Description("All")]
      All = PostOrRepost | Missing
   }
}