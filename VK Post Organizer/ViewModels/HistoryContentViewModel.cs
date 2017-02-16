using Prism.Events;
using Prism.Mvvm;

namespace vk.ViewModels {
   public class HistoryContentViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;

      public HistoryContentViewModel(IEventAggregator eventAggregator) {
         _eventAggregator = eventAggregator;
      }
   }
}