using JetBrains.Annotations;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Infrastructure;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class ContentViewModel : BindableBase, INavigationAware {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;
      private bool _isOverlayVisible;

      public bool IsOverlayVisible {
         get { return _isOverlayVisible; }
         set { SetProperty(ref _isOverlayVisible, value); }
      }

      public ContentViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) {
         _eventAggregator = eventAggregator;
         _regionManager = regionManager;
      }

      public void OnNavigatedTo(NavigationContext navigationContext) {
         if (App.IsInitialized) {
            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, ViewNames.WallPostponeContent);
         }
      }

      public bool IsNavigationTarget(NavigationContext navigationContext) {
         return true;
      }

      public void OnNavigatedFrom(NavigationContext navigationContext) {
      }
   }
}