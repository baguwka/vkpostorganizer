using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace vk.ViewModels {
   public class HistoryContentViewModel : BindableBase, INavigationAware {
      private readonly IEventAggregator _eventAggregator;
      private string _message;

      public string Message {
         get { return _message; }
         set { SetProperty(ref _message, value); }
      }

      public HistoryContentViewModel(IEventAggregator eventAggregator) {
         _eventAggregator = eventAggregator;
      }

      public void OnNavigatedTo(NavigationContext navigationContext) {
         var filter = (string)navigationContext.Parameters["filter"];
         if (!string.IsNullOrEmpty(filter)) {
            Message = filter;
         }
      }

      public bool IsNavigationTarget(NavigationContext navigationContext) {
         return true;
      }

      public void OnNavigatedFrom(NavigationContext navigationContext) {
      }
   }
}