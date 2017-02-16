using Prism.Events;
using Prism.Mvvm;

namespace vk.ViewModels {
   public class WallPostponeContentViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;

      public WallPostponeContentViewModel(IEventAggregator eventAggregator) {
         _eventAggregator = eventAggregator;
      }
   }
}