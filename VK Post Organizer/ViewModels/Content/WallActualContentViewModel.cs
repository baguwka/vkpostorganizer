using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using vk.Models;
using vk.Models.Pullers;
using vk.Models.VkApi.Entities;
using vk.Utils;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallActualContentViewModel : WallContentViewModel {
      private readonly VkPostViewModelBuilder _postBuilder;

      public WallActualContentViewModel(IEventAggregator eventAggregator, PullersController pullersController, 
         VkPostViewModelBuilder postBuilder) 
         : base(eventAggregator, pullersController) {

         _postBuilder = postBuilder;
         _pullersController.Actual.PullInvoked += onActualPullInvoked;
         _pullersController.Actual.PullCompleted += onActualPullCompleted;

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
      }

      protected override async Task buildViewModelPosts(IEnumerable<IPost> posts) {
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

         vms.Sort((a, b) => b.Post.Date.CompareTo(a.Post.Date));

         UnfilteredItems.Clear();
         UnfilteredItems.AddRange(vms);
         filterOut(vms);
      }

      private void onActualPullInvoked(object sender, EventArgs e) {

      }

      private async void onActualPullCompleted(object sender, ContentPullerEventArgs e) {
         if (!IsActive) return;
         Debug.WriteLine($"Actual vm pull completed event listener. Successful? {e.Successful}");
         if (e.Successful) {
            await syncAsync(e.Items);
         }
      }

      public override async void OnNavigatedTo(NavigationContext navigationContext) {
         base.OnNavigatedTo(navigationContext);

         if (LastTimeSynced < _pullersController.Actual.LastTimePulled) {
            Debug.WriteLine("Actual NavigatedTo callback");
            IsBusy = true;
            try {
               LastTimeSynced = DateTimeOffset.Now;
               await buildViewModelPosts(_pullersController.Actual.Items);
            }
            finally {
               IsBusy = false;
            }
         }
      }
   }
}