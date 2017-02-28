﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using vk.Models;
using vk.Models.Pullers;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class HistoryContentViewModel : WallContentViewModel {
      public HistoryContentViewModel(IEventAggregator eventAggregator, PullersController pullersController, 
         SharedWallContext sharedWallContext) 
         : base(eventAggregator, pullersController, sharedWallContext) {

         _pullersController.History.PullInvoked += onHistoryPullerPullInvoked;
         _pullersController.History.PullCompleted += onHistoryPullerPullCompleted;

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
         Debug.WriteLine($"GOOD THREAD IS {Thread.CurrentThread.ManagedThreadId}");
      }

      protected override async void onIsActiveChanged(object sender, EventArgs eventArgs) {
         base.onIsActiveChanged(sender, eventArgs);

         //if (!IsActive) return;

         //if (LastTimeSynced < _pullersController.History.LastTimePulled) {
         //   LastTimeSynced = _pullersController.History.LastTimePulled;
         //   IsBusy = true;
         //   try {
         //      await filterOutAsync(_pullersController.History.Items);
         //   }
         //   finally {
         //      IsBusy = false;
         //   }
         //}
      }

      protected void buildViewModelPosts(IEnumerable<IPost> posts) {
         clear(UnfilteredItems);
         clear(FilteredItems);

         Debug.WriteLine($"--- HISTORY BUILD POSTS. THREAD {Thread.CurrentThread.ManagedThreadId}");
         var builder = new HistoryPostViewModelBuilder();
         var vms = builder.Build(posts).ToList();
         UnfilteredItems.Clear();
         UnfilteredItems.AddRange(vms);
         filterOut(vms);
      }

      private void onHistoryPullerPullInvoked(object sender, EventArgs e) {
         //IsBusy = true;
      }

      private void onHistoryPullerPullCompleted(object sender, ContentPullerEventArgs e) {
         if (!IsActive || IsBusy) return;
         Debug.WriteLine($"History vm pull completed event listener. Successful? {e.Successful}. THREAD {Thread.CurrentThread.ManagedThreadId}");
         if (e.Successful) {
            IsBusy = true;
            try {
               LastTimeSynced = DateTimeOffset.Now;
               buildViewModelPosts(e.Items);
            }
            finally {
               IsBusy = false;
            }
         }
      }

      protected override void updateFilter() {
         base.updateFilter();

         _eventAggregator.GetEvent<FlagsChangedEvent>().Publish(CurrentPostFilter.CompositePostType);
      }
      public override void OnNavigatedTo(NavigationContext navigationContext) {
         base.OnNavigatedTo(navigationContext);

         if (LastTimeSynced < _pullersController.History.LastTimePulled) {
            Debug.WriteLine("History NavigatedTo callback");
            IsBusy = true;
            try {
               LastTimeSynced = DateTimeOffset.Now;
               buildViewModelPosts(_pullersController.History.Items);
               //_pullersController.SharedWallHolder = _sharedWallContext.SelectedWallHolder;
               //await _pullersController.History.PullAsync();
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