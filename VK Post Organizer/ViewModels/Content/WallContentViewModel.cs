using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Events;
using vk.Models;
using vk.Models.Filter;
using vk.Models.Pullers;
using vk.Utils;

namespace vk.ViewModels {
   public abstract class WallContentViewModel : BindableBase, INavigationAware, IActiveAware {
      protected readonly IEventAggregator _eventAggregator;
      protected readonly PullersController _pullersController;
      protected readonly SharedWallContext _sharedWallContext;

      private bool _isBusy;
      private bool _filterPostsIsChecked;
      private bool _filterRepostsIsChecked;
      private IList <PostViewModelBase> _filteredItems;

      public bool IsBusy {
         get { return _isBusy; }
         set { SetProperty(ref _isBusy, value);
            _eventAggregator.GetEvent<ContentEvents.BusyEvent>().Publish(_isBusy);
         }
      }

      public bool FilterPostsIsChecked {
         get { return _filterPostsIsChecked; }
         set { SetProperty(ref _filterPostsIsChecked, value); }
      }

      public bool FilterRepostsIsChecked {
         get { return _filterRepostsIsChecked; }
         set { SetProperty(ref _filterRepostsIsChecked, value); }
      }

      public IList<PostViewModelBase> FilteredItems {
         get { return _filteredItems; }
         private set { SetProperty(ref _filteredItems, value); }
      }

      public CompositePostFilter CurrentPostFilter { get; }

      public ICommand PostFilterCheckedCommand { get; protected set; }
      public ICommand PostFilterUncheckedCommand { get; protected set; }

      public ICommand RepostFilterCheckedCommand { get; protected set; }
      public ICommand RepostFilterUncheckedCommand { get; protected set; }

      public WallContentViewModel(IEventAggregator eventAggregator, PullersController pullersController, SharedWallContext sharedWallContext) {
         _eventAggregator = eventAggregator;
         _pullersController = pullersController;
         _sharedWallContext = sharedWallContext;
         CurrentPostFilter = new CompositePostFilter();
         FilteredItems = new RangeObservableCollection<PostViewModelBase>();

         FilterPostsIsChecked = true;
         FilterRepostsIsChecked = true;


         _eventAggregator.GetEvent<ContentEvents.LeftBlockExpandAllRequest>().Subscribe(expandAllItems);
         _eventAggregator.GetEvent<ContentEvents.LeftBlockCollapseAllRequest>().Subscribe(collapseAllItems);
      }

      protected virtual void expandAllItems() {
         if (!IsActive) return;
         FilteredItems.ForEach(i => i.Expand());
      }

      protected virtual void collapseAllItems() {
         if (!IsActive) return;
         FilteredItems.ForEach(i => i.Collapse());
      }

      public virtual void OnNavigatedTo(NavigationContext navigationContext) {
         updateFilter();
      }

      public virtual bool IsNavigationTarget(NavigationContext navigationContext) {
         return !IsBusy;
      }

      public virtual void OnNavigatedFrom(NavigationContext navigationContext) {

      }

      protected async Task filterOutAsync(IEnumerable<PostViewModelBase> items) {
         IsBusy = true;
         try {
            var currentFiltered = _filteredItems;

            IEnumerable<PostViewModelBase> tempItems = new List<PostViewModelBase>();
            await Task.Run(() => tempItems = items.Where(CurrentPostFilter.Suitable));

            this.FilteredItems = null;

            currentFiltered.Clear();

            foreach (var postControl in tempItems) {
               currentFiltered.Add(postControl);
            }

            FilteredItems = currentFiltered;
         }
         finally {
            IsBusy = false;
         }
      }

      protected virtual void updateFilter() {
         if (FilterPostsIsChecked) {
            CurrentPostFilter.CompositePostType = CurrentPostFilter.CompositePostType | PostType.Post;
         }
         else {
            CurrentPostFilter.CompositePostType &= ~PostType.Post;
         }

         if (FilterRepostsIsChecked) {
            CurrentPostFilter.CompositePostType = CurrentPostFilter.CompositePostType | PostType.Repost;
         }
         else {
            CurrentPostFilter.CompositePostType &= ~PostType.Repost;
         }
      }

      public bool IsActive { get; set; }
      public event EventHandler IsActiveChanged;
   }
}