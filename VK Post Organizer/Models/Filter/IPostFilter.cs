using System.Collections.Generic;
using vk.ViewModels;

namespace vk.Models.Filter {
   public interface IPostFilter {
      IEnumerable<PostItem> FilterPosts(IEnumerable<PostItem> postItems);
   }
}