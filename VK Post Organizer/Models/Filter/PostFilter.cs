using System.Collections.Generic;
using System.Linq;
using vk.ViewModels;

namespace vk.Models.Filter {
   public abstract class PostFilter {
      public IEnumerable<PostControl> FilterPosts(IEnumerable<PostControl> postItems) {
         return postItems.Where(Suitable);
      }

      public abstract bool Suitable(PostControl postControl);
   }
}