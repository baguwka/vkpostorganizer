using System.Collections.Generic;
using vk.ViewModels;

namespace vk.Models.Filter {
   public interface IPostFilter {
      IEnumerable<PostControl> FilterPosts(IEnumerable<PostControl> postItems);
   }
}