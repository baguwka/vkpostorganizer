using vk.ViewModels;

namespace vk.Models.Filter {
   public class NoPostFilter : PostFilter {
      public static PostFilter Instance { get; }

      static NoPostFilter() {
         Instance = new NoPostFilter();
      }

      public override bool Suitable(IPostType postControl) {
         return true;
      }
   }

   public class MissingPostFilter : PostFilter {
      public static PostFilter Instance { get; }

      static MissingPostFilter() {
         Instance = new MissingPostFilter();
      }

      public override bool Suitable(IPostType postControl) {
         return postControl.PostType == PostType.Missing;
      }
   }

   public class PostsAndRepostsFilter : PostFilter {
      public override bool Suitable(IPostType postControl) {
         return postControl.PostType == PostType.Post || postControl.PostType == PostType.Repost;
      }
   }

   public class PostsOnlyFilter : PostFilter {
      public override bool Suitable(IPostType postControl) {
         return postControl.PostType == PostType.Post;
      }
   }

   public class RepostsOnlyFilter : PostFilter {
      public override bool Suitable(IPostType postControl) {
         return postControl.PostType == PostType.Repost;
      }
   }

   public class CompositePostFilter : PostFilter {
      public PostType CompositePostType { get; set; }

      public CompositePostFilter() {
      }

      public CompositePostFilter(PostType postType) {
         this.CompositePostType = postType;
      }

      public override bool Suitable(IPostType postControl) {
         return CompositePostType.HasFlag(postControl.PostType);
      }
   }
}