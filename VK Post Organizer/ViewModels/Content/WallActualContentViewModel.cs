using JetBrains.Annotations;
using Prism.Events;
using vk.Models;
using vk.Models.Pullers;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallActualContentViewModel : WallContentViewModel {
      public WallActualContentViewModel(IEventAggregator eventAggregator, PullersController pullersController, SharedWallContext sharedWallContext) 
         : base(eventAggregator, pullersController, sharedWallContext) {
      }
   }
}