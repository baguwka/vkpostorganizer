using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Infrastructure;
using vk.Views;

namespace vk.ViewModels {
   public class ContentViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;

      public ContentViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) {
         _eventAggregator = eventAggregator;
         _regionManager = regionManager;

         _regionManager.RegisterViewWithRegion(RegionNames.ContentLeftBlockRegion, typeof(WallContentLeftBlockView));
      }
   }
}