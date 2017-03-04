using System.Diagnostics;
using JetBrains.Annotations;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Events;
using vk.Infrastructure;
using vk.Models;
using vk.Models.Pullers;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class ContentViewModel : BindableBase, INavigationAware {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;
      private readonly PullersController _pullersController;
      private readonly SharedWallContext _sharedWallContext;
      private bool _isOverlayVisible;
      private bool _isBusy;

      public bool IsOverlayVisible {
         get { return _isOverlayVisible; }
         set { SetProperty(ref _isOverlayVisible, value); }
      }

      public bool IsBusy {
         get { return _isBusy; }
         set {
            if (SetProperty(ref _isBusy, value)) {
               _eventAggregator.GetEvent<ContentEvents.ContentRegionBusyEvent>().Publish(_isBusy);
            }
         }
      }

      public ContentViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, PullersController pullersController,
         SharedWallContext sharedWallContext) {

         _eventAggregator = eventAggregator;
         _regionManager = regionManager;
         _pullersController = pullersController;
         _sharedWallContext = sharedWallContext;
      }

      public async void OnNavigatedTo(NavigationContext navigationContext) {
         if (App.IsInitialized) {
            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, ViewNames.WallPostponeContent);

            IsBusy = true;
            try {
               _pullersController.SharedWallHolder = _sharedWallContext.SelectedWallHolder;
               await _pullersController.SharedPullAsync();
            }
            finally {
               IsBusy = false;
            }
         }
      }

      public bool IsNavigationTarget(NavigationContext navigationContext) {
         return true;
      }

      public void OnNavigatedFrom(NavigationContext navigationContext) {
      }
   }
}