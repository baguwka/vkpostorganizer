using System.Collections.Generic;
using System.Linq;
using vk.ViewModels;

namespace vk.Models.Filter {
   public class NoPostFilter : IPostFilter {
      public static IPostFilter Instance { get; }

      static NoPostFilter() {
         Instance = new NoPostFilter();
      }

      public IEnumerable<PostControl> FilterPosts(IEnumerable<PostControl> postItems) {
         return postItems;
      }
   }

   public class MissingPostFilter : IPostFilter {
      public IEnumerable<PostControl> FilterPosts(IEnumerable<PostControl> postItems) {
         return postItems.Where(i => i.PostType == PostType.Missing);
      }
   }

   public class PostsAndRepostsFilter : IPostFilter {
      public IEnumerable<PostControl> FilterPosts(IEnumerable<PostControl> postItems) {
         return postItems.Where(i => i.PostType == PostType.Post || i.PostType == PostType.Repost);
      }
   }

   public class PostsOnlyFilter : IPostFilter {
      public IEnumerable<PostControl> FilterPosts(IEnumerable<PostControl> postItems) {
         return postItems.Where(i => i.PostType == PostType.Post);
      }
   }

   public class RepostsOnlyFilter : IPostFilter {
      public IEnumerable<PostControl> FilterPosts(IEnumerable<PostControl> postItems) {
         return postItems.Where(i => i.PostType == PostType.Repost);
      }
   }
}