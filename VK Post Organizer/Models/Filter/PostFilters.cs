using System.Collections.Generic;
using System.Linq;
using vk.ViewModels;

namespace vk.Models.Filter {
   public class NoPostFilter : IPostFilter {
      public static IPostFilter Instance { get; }

      static NoPostFilter() {
         Instance = new NoPostFilter();
      }

      public IEnumerable<PostItem> FilterPosts(IEnumerable<PostItem> postItems) {
         return postItems;
      }
   }

   public class PostsOnlyFilter : IPostFilter {
      public IEnumerable<PostItem> FilterPosts(IEnumerable<PostItem> postItems) {
         return postItems.Where(i => i.PostRef.CopyHistory == null || !i.PostRef.CopyHistory.Any());
      }
   }

   public class RepostsOnlyFilter : IPostFilter {
      public IEnumerable<PostItem> FilterPosts(IEnumerable<PostItem> postItems) {
         return postItems.Where(i => i.PostRef.CopyHistory != null && i.PostRef.CopyHistory.Any());
      }
   }
}