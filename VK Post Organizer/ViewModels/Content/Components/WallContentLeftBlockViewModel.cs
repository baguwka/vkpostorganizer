using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Infrastructure;
using vk.Views;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallContentLeftBlockViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;

      public ICommand ShowActualWallCommand { get; private set; }
      public ICommand ShowPostponeWallCommand { get; private set; }
      public ICommand ShowHistoryCommand { get; private set; }

      public WallContentLeftBlockViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) {
         _eventAggregator = eventAggregator;
         _regionManager = regionManager;

         _regionManager.RegisterViewWithRegion(RegionNames.ContentMainRegion, typeof(WallPostponeContentView));

         ShowActualWallCommand = new DelegateCommand(() => {
            var parameters = new NavigationParameters {{"filter", "howdy"}};

            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, ViewNames.WallActualContent, parameters);
         });

         ShowPostponeWallCommand = new DelegateCommand(() => {
            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, $"{ViewNames.WallPostponeContent}?filter=sayhello");
         });

         ShowHistoryCommand = new DelegateCommand(() => {
            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, $"{ViewNames.HistoryContent}?filter=sayhello");
         });
      }
   }
}