using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Events;
using vk.Models;
using vk.Models.Filter;
using vk.Models.Pullers;
using vk.Utils;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   public abstract class WallContentViewModel : BindableBase, INavigationAware, IActiveAware {
      protected readonly IEventAggregator _eventAggregator;
      protected readonly PullersController _pullersController;
      protected readonly SharedWallContext _sharedWallContext;

      private bool _isBusy;
      private bool _filterPostsIsChecked;
      private bool _filterRepostsIsChecked;
      private List<PostViewModelBase> _unfilteredItems;
      private IList <PostViewModelBase> _filteredItems;
      private bool _isActive;

      public bool IsBusy {
         get { return _isBusy; }
         set {
            if (SetProperty(ref _isBusy, value)) {
               _eventAggregator.GetEvent<ContentEvents.BusyEvent>().Publish(_isBusy);
            }
         }
      }

      public bool FilterPostsIsChecked {
         get { return _filterPostsIsChecked; }
         set {
            if (IsBusy) return;
            SetProperty(ref _filterPostsIsChecked, value);
         }
      }

      public bool FilterRepostsIsChecked {
         get { return _filterRepostsIsChecked; }
         set {
            if (IsBusy) return;
            SetProperty(ref _filterRepostsIsChecked, value);
         }
      }

      public List<PostViewModelBase> UnfilteredItems {
         get { return _unfilteredItems; }
         private set { SetProperty(ref _unfilteredItems, value); }
      }

      public IList<PostViewModelBase> FilteredItems {
         get { return _filteredItems; }
         private set { SetProperty(ref _filteredItems, value); }
      }

      public DateTimeOffset LastTimeSynced { get; protected set; }

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
         UnfilteredItems = new List<PostViewModelBase>();
         FilteredItems = new RangeObservableCollection<PostViewModelBase>();

         FilterPostsIsChecked = true;
         FilterRepostsIsChecked = true;

         _eventAggregator.GetEvent<ContentEvents.LeftBlockExpandAllRequest>().Subscribe(expandAllItems);
         _eventAggregator.GetEvent<ContentEvents.LeftBlockCollapseAllRequest>().Subscribe(collapseAllItems);

         IsActiveChanged += onIsActiveChanged;
      }

      protected virtual void onIsActiveChanged(object sender, EventArgs eventArgs) {

      }
      
      //todo: put this somewhere else
      public bool IsDateMatchTheSchedule(int unixTime, Schedule schedule) {
         var dateTime = UnixTimeConverter.ToDateTime(unixTime);
         return schedule.Items.Any(i => i.Hour == dateTime.Hour && i.Minute == dateTime.Minute);
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

      protected virtual void clear(IEnumerable<PostViewModelBase> posts) {
         foreach (var postViewModelBase in posts) {
            clearPost(postViewModelBase);
         }
      }

      protected virtual void clearPost(PostViewModelBase post) {
         post.ClearPreview();
      }

      protected abstract Task buildViewModelPosts(IEnumerable<IPost> posts);

      protected virtual async Task syncAsync(IEnumerable<IPost> items) {
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

      protected void filterOut(IEnumerable<PostViewModelBase> items) {
         var currentFiltered = _filteredItems;
         var tempItems = items.Where(CurrentPostFilter.Suitable);

         this.FilteredItems = null;

         currentFiltered.Clear();

         foreach (var postControl in tempItems) {
            currentFiltered.Add(postControl);
         }

         FilteredItems = currentFiltered;
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

      public bool IsActive {
         get { return _isActive; }
         set {
            _isActive = value;
            OnIsActiveChanged();
         }
      }

      public event EventHandler IsActiveChanged;

      protected virtual void OnIsActiveChanged() {
         IsActiveChanged?.Invoke(this, EventArgs.Empty);
      }
   }
}