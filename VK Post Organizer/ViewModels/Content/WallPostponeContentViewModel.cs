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
   [UsedImplicitly]
   public class WallPostponeContentViewModel : WallContentViewModel {
      private readonly VkPostViewModelBuilder _postBuilder;
      private bool _filterMissingIsChecked;

      public bool FilterMissingIsChecked {
         get { return _filterMissingIsChecked; }
         set {
            if (IsBusy) return;
            SetProperty(ref _filterMissingIsChecked, value);
         }
      }

      public ICommand MissingFilterCheckedCommand { get; private set; }
      public ICommand MissingFilterUncheckedCommand { get; private set; }

      public WallPostponeContentViewModel(IEventAggregator eventAggregator, PullersController pullersController, VkPostViewModelBuilder postBuilder)
         : base(eventAggregator, pullersController) {
         _postBuilder = postBuilder;

         FilterMissingIsChecked = true;

         _pullersController.Postponed.PullInvoked += onPostponedPullInvoked;
         _pullersController.Postponed.PullCompleted += onPostponedPullCompleted;
         //_pullersController.Postponed.UploadRequested += onVkPullerUploadRequest;

         PostFilterCheckedCommand = new DelegateCommand(() => {
               updateFilter();
               filterOut(UnfilteredItems);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         PostFilterUncheckedCommand = new DelegateCommand(() => {
               updateFilter();
               filterOut(UnfilteredItems);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterCheckedCommand = new DelegateCommand(() => {
               updateFilter();
               filterOut(UnfilteredItems);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterUncheckedCommand = new DelegateCommand(() => {
               updateFilter();
               filterOut(UnfilteredItems);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         MissingFilterCheckedCommand = new DelegateCommand(() => {
               updateFilter();
               filterOut(UnfilteredItems);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         MissingFilterUncheckedCommand = new DelegateCommand(() => {
               updateFilter();
               filterOut(UnfilteredItems);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);
      }

      protected override async Task buildViewModelPosts(IEnumerable<IPost> posts) {
         var sw = Stopwatch.StartNew();
         clear(UnfilteredItems);
         clear(FilteredItems);

         var rawPosts = posts as IList<IPost> ?? posts.ToList();
         var schedule = new Schedule();

         var vms = new List<VkPostViewModel>();
         var freshViewModels = await _postBuilder.BuildAsync(rawPosts);
         vms.AddRange(freshViewModels.Cast<VkPostViewModel>());

         vms.Where(post => {
            var vmPost = post;
            if (vmPost == null) return false;
            return IsDateMatchTheSchedule(vmPost.Post.Date, schedule);
         }).ForEach(post => {
            var vmPost = post;
            if (vmPost == null) return;
            vmPost.Mark = PostMark.Good;
         });

#pragma warning disable 4014
         _postBuilder.GetOwnerNamesOfPosts(vms.Where(post => post.PostType == PostType.Repost).ToList());
#pragma warning restore 4014

         var missingFiller = new MissingFiller();
         var missingDates = await missingFiller.GetMissingDates(rawPosts, schedule);
         var missingPosts = missingFiller.BuildMissingViewModels(missingDates).ToList();

         missingPosts.ForEach(p => p.UploadRequested += onMissingPostRequestedUpload);

         vms.AddRange(missingPosts);
         vms.Sort((a, b) => a.Post.Date.CompareTo(b.Post.Date));

         UnfilteredItems.Clear();
         UnfilteredItems.AddRange(vms);
         filterOut(vms);
         sw.Stop();
         Debug.WriteLine($"Потребовалось времени для билда: {sw.ElapsedMilliseconds}");
      }

      protected override void clearPost(PostViewModelBase post) {
         base.clearPost(post);
         var vkPost = post as VkPostViewModel;
         if (vkPost == null) return;
         vkPost.UploadRequested -= onMissingPostRequestedUpload;
      }

      private void onMissingPostRequestedUpload(object sender, EventArgs eventArgs) {
         var post = sender as VkPostViewModel;
         if (post == null) return;

         _eventAggregator.GetEvent<UploaderEvents.Configure>().Publish(new UploaderViewModelConfiguration() {
            IsEnabled = true,
            DateOverride = post.Post.Date
         });
         _eventAggregator.GetEvent<UploaderEvents.SetVisibility>().Publish(true);
      }

      protected async void onPostponedPullCompleted(object sender, ContentPullerEventArgs e) {
         if (!IsActive) return;
         Debug.WriteLine($"Postpone vm pull completed event listener. Successful? {e.Successful}");
         if (e.Successful) {
            await syncAsync(e.Items);
         }
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
      }

      public override async void OnNavigatedTo(NavigationContext navigationContext) {
         base.OnNavigatedTo(navigationContext);

         if (LastTimeSynced < _pullersController.Postponed.LastTimePulled) {
            Debug.WriteLine("Postpone NavigatedTo callback");
            IsBusy = true;
            try {
               LastTimeSynced = DateTimeOffset.Now;
               await buildViewModelPosts(_pullersController.Postponed.Items);
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