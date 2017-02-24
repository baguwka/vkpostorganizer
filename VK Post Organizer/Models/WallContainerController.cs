using JetBrains.Annotations;
using Prism.Events;
using vk.Events;
using vk.Models.Filter;

namespace vk.Models {
   [UsedImplicitly]
   public class WallContainerController {
      private readonly IEventAggregator _eventAggregator;
      public WallContainer Container { get; }

      public WallContainerController(IEventAggregator eventAggregator, WallContainer wallContainer) {
         _eventAggregator = eventAggregator;
         Container = wallContainer;

         _eventAggregator.GetEvent<MainBottomEvents.Refresh>().Subscribe(async () => {
            await Container.PullWithScheduleHightlightAsync(new NoPostFilter(), new Schedule());
         });
      }
   }
}