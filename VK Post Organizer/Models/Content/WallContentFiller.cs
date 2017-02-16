using Prism.Mvvm;
using vk.Models.VkApi.Entities;

namespace vk.Models.Content {
   public class WallContent : BindableBase {
      private SmartCollection<Post> _wallPosts;

      public SmartCollection<Post> WallPosts {
         get { return _wallPosts; }
         private set { SetProperty(ref _wallPosts, value); }
      }

      public WallContent() {
         WallPosts = new SmartCollection<Post>();
      }
   }

   public class WallContentFiller {
      
   }
}