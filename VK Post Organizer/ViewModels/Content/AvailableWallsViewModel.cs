using System;
using System.Windows;
using JetBrains.Annotations;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using vk.Events;
using vk.Models;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class AvailableWallsViewModel : BindableBase, IActiveAware {
      private readonly IEventAggregator _eventAggregator;
      private readonly AvailableWallsFiller _filler;
      private readonly SharedWallContext _sharedWallContext;
      private WallList _wallList;

      public WallList WallList {
         get { return _wallList; }
         set { SetProperty(ref _wallList, value); }
      }

      public bool IsActive { get; set; }
      public event EventHandler IsActiveChanged;

      private bool _isAuthorized;

      public bool IsAuthorized {
         get { return _isAuthorized; }
         set { SetProperty(ref _isAuthorized, value); }
      }

      public AvailableWallsViewModel(IEventAggregator eventAggregator, AvailableWallsFiller filler, SharedWallContext sharedWallContext) {
         _eventAggregator = eventAggregator;
         _filler = filler;
         _sharedWallContext = sharedWallContext;
         WallList = new WallList();

         WallList.ItemClicked += onWallItemClicked;
         _eventAggregator.GetEvent<WallSelectorEvents.FillWallRequest>().Subscribe(fillWallList);
         _eventAggregator.GetEvent<MainBottomEvents.Refresh>().Subscribe(fillWallList);

         _eventAggregator.GetEvent<AuthBarEvents.AuthorizationCompleted>()
            .Subscribe(authorized => IsAuthorized = authorized);
         _eventAggregator.GetEvent<AuthBarEvents.LogOutCompleted>()
            .Subscribe(() => IsAuthorized = false);
      }

      private void onWallItemClicked(object sender, WallItem e) {
         if (!IsAuthorized) return;

         _sharedWallContext.SelectedWallHolder = e.WallHolder;
         _eventAggregator.GetEvent<WallSelectorEvents.WallSelected>().Publish(e);
      }

      private async void fillWallList() {
         if (!IsActive) return;

         var result = await _filler.FillAsync(_wallList);
         if (result.Succeed == false) {
            MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }
   }
}