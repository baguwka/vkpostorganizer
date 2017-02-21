using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Infrastructure;
using vk.Models;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallContentLeftBlockViewModel : BindableBase, INavigationAware {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;
      private readonly VkUploader _uploader;
      private string _description;
      private string _profilePhoto;
      private string _name;
      private string _infoPanel;

      public ICommand ShowActualWallCommand { get; private set; }
      public ICommand ShowPostponeWallCommand { get; private set; }
      public ICommand ShowHistoryCommand { get; private set; }
      public ICommand ExpandAllCommand { get; private set; }
      public ICommand CollapseAllCommand { get; private set; }

      public string Description {
         get { return _description; }
         set { SetProperty(ref _description, value); }
      }

      public string ProfilePhoto {
         get { return _profilePhoto; }
         set { SetProperty(ref _profilePhoto, value); }
      }

      public string Name {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      public string InfoPanel {
         get { return _infoPanel; }
         set { SetProperty(ref _infoPanel, value); }
      }

      public WallContentLeftBlockViewModel(IEventAggregator eventAggregator, IRegionManager regionManager,
         VkUploader uploader) {
         _eventAggregator = eventAggregator;
         _regionManager = regionManager;
         _uploader = uploader;

         ShowActualWallCommand = new DelegateCommand(() => {
            var parameters = new NavigationParameters {{"filter", "howdy"}};
            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, ViewNames.WallActualContent, parameters);
         });

         ShowPostponeWallCommand =
            new DelegateCommand(
               () => {
                  _regionManager.RequestNavigate(RegionNames.ContentMainRegion,
                     $"{ViewNames.WallPostponeContent}?filter=sayhello");
               });

         ShowHistoryCommand =
            new DelegateCommand(
               () => {
                  _regionManager.RequestNavigate(RegionNames.ContentMainRegion,
                     $"{ViewNames.HistoryContent}?filter=sayhello");
               });
      }

      public void OnNavigatedTo(NavigationContext navigationContext) {
      }

      public bool IsNavigationTarget(NavigationContext navigationContext) {
         return true;
      }

      public void OnNavigatedFrom(NavigationContext navigationContext) {
      }
   }
}