using JetBrains.Annotations;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class ContentViewModel : BindableBase {
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
   }
}