using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace vk.ViewModels {
   public class ContentViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;

      public ContentViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) {
         _eventAggregator = eventAggregator;
         _regionManager = regionManager;

      }
   }
}