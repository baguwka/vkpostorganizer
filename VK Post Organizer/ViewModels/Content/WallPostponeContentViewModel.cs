using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using vk.Events;
using vk.Models;
using vk.Models.Pullers;
using vk.Models.VkApi.Entities;
using vk.Utils;

namespace vk.ViewModels {
   public class FlagsChangedEvent : PubSubEvent<PostType> { }

   [UsedImplicitly]
   public class WallPostponeContentViewModel : WallContentViewModel {
      private readonly VkPostViewModelBuilder _postBuilder;
      private bool _filterMissingIsChecked;

      public bool FilterMissingIsChecked {
         get { return _filterMissingIsChecked; }
         set { SetProperty(ref _filterMissingIsChecked, value); }
      }

      public ICommand MissingFilterCheckedCommand { get; private set; }
      public ICommand MissingFilterUncheckedCommand { get; private set; }

      public WallPostponeContentViewModel(IEventAggregator eventAggregator, PullersController pullersController,
         SharedWallContext sharedWallContext, VkPostViewModelBuilder postBuilder)
         : base(eventAggregator, pullersController, sharedWallContext) {
         _postBuilder = postBuilder;

         FilterMissingIsChecked = true;

         _pullersController.Postponed.PullInvoked += onPostponedPullInvoked;
         _pullersController.Postponed.PullCompleted += onPostponedPullCompleted;
         //_pullersController.Postponed.UploadRequested += onVkPullerUploadRequest;

         PostFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
               updateFilter();
               await syncAsync(_pullersController.Postponed.Items);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         PostFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
               updateFilter();
               await syncAsync(_pullersController.Postponed.Items);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
               updateFilter();
               await syncAsync(_pullersController.Postponed.Items);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
               updateFilter();
               await syncAsync(_pullersController.Postponed.Items);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         MissingFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
               updateFilter();
               await syncAsync(_pullersController.Postponed.Items);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         MissingFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
               updateFilter();
               await syncAsync(_pullersController.Postponed.Items);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);
      }

      protected async Task buildViewModelPosts(IEnumerable<IPost> posts) {
         var schedule = new Schedule();

         var vms = new List<VkPostViewModel>();
         var rawPosts = await _postBuilder.BuildAsync(posts);
         vms.AddRange(rawPosts.Cast<VkPostViewModel>());
         vms.Where(post => {
            var vmPost = post;
            if (vmPost == null) return false;
            return IsDateMatchTheSchedule(vmPost.Post.Date, schedule);

         }).ForEach(post => {
            var vmPost = post;
            if (vmPost == null) return;
            vmPost.Mark = PostMark.Good;
         });

         var missingFiller = new MissingFiller();
         var missingDates = await missingFiller.GetMissingDates(posts, schedule);
         var missingPosts = missingFiller.BuildMissingViewModels(missingDates);

         vms.AddRange(missingPosts);
         vms.Sort((a, b) => a.Post.Date.CompareTo(b.Post.Date));

         filterOut(vms);
      }

      public bool IsDateMatchTheSchedule(int unixTime, Schedule schedule) {
         var dateTime = UnixTimeConverter.ToDateTime(unixTime);
         return schedule.Items.Any(i => i.Hour == dateTime.Hour && i.Minute == dateTime.Minute);
      }

      protected override async void onIsActiveChanged(object sender, EventArgs eventArgs) {
         base.onIsActiveChanged(sender, eventArgs);

         //if (!IsActive) return;

         //if (LastTimeSynced < _pullersController.Vk.LastTimePulled) {
         //   LastTimeSynced = _pullersController.Vk.LastTimePulled;
         //   IsBusy = true;
         //   try {
         //      await filterOutAsync(_pullersController.Vk.Items);
         //   }
         //   finally {
         //      IsBusy = false;
         //   }
         //}
      }

      protected async void onPostponedPullCompleted(object sender, ContentPullerEventArgs e) {
         if (!IsActive) return;
         Debug.WriteLine($"Postpone vm pull completed event listener. Successful? {e.Successful}");
         if (e.Successful) {
            await syncAsync(e.Items);
         }
      }

      private async Task syncAsync(IEnumerable<IPost> items) {
         if (IsBusy) return;

         IsBusy = true;
         try {
            LastTimeSynced = DateTimeOffset.Now;
            await buildViewModelPosts(items);
         }
         finally {
            IsBusy = false;
         }
      }

      private void onVkPullerUploadRequest(object sender, VkPostViewModel vkPostViewModel) {
         _eventAggregator.GetEvent<UploaderEvents.Configure>().Publish(new UploaderViewModelConfiguration() {
            IsEnabled = true,
            DateOverride = vkPostViewModel.Post.Date
         });
         _eventAggregator.GetEvent<UploaderEvents.SetVisibility>().Publish(true);
      }

      private void onPostponedPullInvoked(object sender, EventArgs eventArgs) {
         //IsBusy = true;
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

         if (LastTimeSynced < _pullersController.Postponed.LastTimePulled) {
            Debug.WriteLine("Postpone NavigatedTo callback");
            IsBusy = true;
            try {
               buildViewModelPosts(_pullersController.Postponed.Items);
               LastTimeSynced = DateTimeOffset.Now;
               //_pullersController.SharedWallHolder = _sharedWallContext.SelectedWallHolder;
               //await _pullersController.Vk.PullWithScheduleHightlightAsync(new NoPostFilter(), new Schedule());
            }
            finally {
               IsBusy = false;
            }
         }
      }

      public override void OnNavigatedFrom(NavigationContext navigationContext) {
         base.OnNavigatedFrom(navigationContext);
      }
   }
}