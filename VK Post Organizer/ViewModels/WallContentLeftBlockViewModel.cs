using Prism.Events;
using Prism.Mvvm;

namespace vk.ViewModels {
   public class WallContentLeftBlockViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;

      public WallContentLeftBlockViewModel(IEventAggregator eventAggregator) {
         _eventAggregator = eventAggregator;
      }
   }
}