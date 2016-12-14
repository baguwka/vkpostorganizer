namespace vk.Models {
   public enum PostType {
      Post,
      Repost,
      Both = Post | Repost
   }
}