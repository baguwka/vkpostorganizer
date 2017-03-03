using System.Collections.ObjectModel;
using Prism.Mvvm;
using vk.Models.VkApi.Entities;

namespace vk.Models.Content {
   public class WallContent : BindableBase {
      private ObservableCollection<Post> _wallPosts;

      public ObservableCollection<Post> WallPosts {
         get { return _wallPosts; }
         private set { SetProperty(ref _wallPosts, value); }
      }

      public WallContent() {
         WallPosts = new ObservableCollection<Post>();
      }
   }

   public class WallContentFiller {
      
   }
}