using JetBrains.Annotations;
using Prism.Events;
using vk.Models;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallActualContentViewModel : WallContentViewModel {
      public WallActualContentViewModel(IEventAggregator eventAggregator, WallContainerController wallContainerController, SharedWallContext sharedWallContext) 
         : base(eventAggregator, wallContainerController, sharedWallContext) {
      }
   }
}