using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using vk.Events;
using vk.Models;
using vk.Models.Pullers;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class HistoryContentViewModel : WallContentViewModel {
      public HistoryContentViewModel(IEventAggregator eventAggregator, PullersController pullersController, 
         SharedWallContext sharedWallContext) 
         : base(eventAggregator, pullersController, sharedWallContext) {

         _pullersController.History.PullInvoked += onHistoryPullerPullInvoked;
         _pullersController.History.PullCompleted += onHistoryPullerPullCompleted;

         PostFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.History.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         PostFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.History.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.History.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.History.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);
         //todo: refactor HistoryPuller with VkPuller
      }

      private void onHistoryPullerPullInvoked(object sender, EventArgs e) {
         IsBusy = true;
      }

      private async void onHistoryPullerPullCompleted(object sender, IList<HistoryPostViewModel> items) {
         IsBusy = true;
         try {
            await filterOutAsync(items);
         }
         finally {
            IsBusy = false;
         }
      }

      protected override void updateFilter() {
         base.updateFilter();

         _eventAggregator.GetEvent<FlagsChangedEvent>().Publish(CurrentPostFilter.CompositePostType);
      }

      public override async void OnNavigatedTo(NavigationContext navigationContext) {
         base.OnNavigatedTo(navigationContext);

         IsBusy = true;
         try {
            await filterOutAsync(_pullersController.History.Items);
            //_pullersController.SharedWallHolder = _sharedWallContext.SelectedWallHolder;
            //await _pullersController.History.PullAsync();
         }
         finally {
            IsBusy = false;
         }
      }

      public override void OnNavigatedFrom(NavigationContext navigationContext) {
         base.OnNavigatedFrom(navigationContext);
      }
   }
}