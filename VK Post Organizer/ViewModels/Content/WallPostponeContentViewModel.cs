using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using vk.Events;
using vk.Models;
using vk.Models.Filter;
using vk.Models.Pullers;

namespace vk.ViewModels {
   public class FlagsChangedEvent : PubSubEvent<PostType> { }

   [UsedImplicitly]
   public class WallPostponeContentViewModel : WallContentViewModel {
      private bool _filterMissingIsChecked;

      public bool FilterMissingIsChecked {
         get { return _filterMissingIsChecked; }
         set { SetProperty(ref _filterMissingIsChecked, value); }
      }

      public ICommand MissingFilterCheckedCommand { get; private set; }
      public ICommand MissingFilterUncheckedCommand { get; private set; }

      public WallPostponeContentViewModel(IEventAggregator eventAggregator, PullersController pullersController, 
         SharedWallContext sharedWallContext) : base(eventAggregator, pullersController, sharedWallContext) {

         FilterMissingIsChecked = true;

         _pullersController.Vk.PullInvoked += onVkPullerPullInvoked;
         _pullersController.Vk.PullCompleted += onVkPullerPullCompleted;
         _pullersController.Vk.UploadRequested += onVkPullerUploadRequest;

         PostFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.Vk.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         PostFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.Vk.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.Vk.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.Vk.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         MissingFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.Vk.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         MissingFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_pullersController.Vk.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);
      }

      protected async void onVkPullerPullCompleted(object sender, IList<PostViewModel> items) {
         IsBusy = true;
         try {
            await filterOutAsync(items);
         }
         finally {
            IsBusy = false;
         }
      }

      private void onVkPullerUploadRequest(object sender, PostViewModel postViewModel) {
         _eventAggregator.GetEvent<UploaderEvents.Configure>().Publish(new UploaderViewModelConfiguration() {
            IsEnabled = true,
            DateOverride = postViewModel.Post.DateUnix
         });
         _eventAggregator.GetEvent<UploaderEvents.SetVisibility>().Publish(true);
      }

      private void onVkPullerPullInvoked(object sender, EventArgs eventArgs) {
         IsBusy = true;
      }

      protected override void updateFilter() {
         if (FilterMissingIsChecked) {
            CurrentPostFilter.CompositePostType = CurrentPostFilter.CompositePostType | PostType.Missing;
         }
         else {
            CurrentPostFilter.CompositePostType &= ~PostType.Missing;
         }
         base.updateFilter();

         _eventAggregator.GetEvent<FlagsChangedEvent>().Publish(CurrentPostFilter.CompositePostType);
      }

      public override async void OnNavigatedTo(NavigationContext navigationContext) {
         base.OnNavigatedTo(navigationContext);

         IsBusy = true;
         try {
            await filterOutAsync(_pullersController.Vk.Items);
            //_pullersController.SharedWallHolder = _sharedWallContext.SelectedWallHolder;
            //await _pullersController.Vk.PullWithScheduleHightlightAsync(new NoPostFilter(), new Schedule());
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