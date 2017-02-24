using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Events;
using vk.Models;
using vk.Models.Filter;

namespace vk.ViewModels {
   public abstract class WallContentViewModel : BindableBase, INavigationAware {
      protected readonly IEventAggregator _eventAggregator;
      protected readonly WallContainerController _wallContainerController;
      protected readonly SharedWallContext _sharedWallContext;

      private bool _isBusy;
      private bool _filterPostsIsChecked;
      private bool _filterRepostsIsChecked;
      private IList <PostControl> _filteredItems;

      public bool IsBusy {
         get { return _isBusy; }
         set { SetProperty(ref _isBusy, value); }
      }

      public bool FilterPostsIsChecked {
         get { return _filterPostsIsChecked; }
         set { SetProperty(ref _filterPostsIsChecked, value); }
      }

      public bool FilterRepostsIsChecked {
         get { return _filterRepostsIsChecked; }
         set { SetProperty(ref _filterRepostsIsChecked, value); }
      }

      public IList<PostControl> FilteredItems {
         get { return _filteredItems; }
         private set { SetProperty(ref _filteredItems, value); }
      }

      public CompositePostFilter CurrentPostFilter { get; }

      public ICommand PostFilterCheckedCommand { get; private set; }
      public ICommand PostFilterUncheckedCommand { get; private set; }

      public ICommand RepostFilterCheckedCommand { get; private set; }
      public ICommand RepostFilterUncheckedCommand { get; private set; }

      public WallContentViewModel(IEventAggregator eventAggregator, WallContainerController wallContainerController, SharedWallContext sharedWallContext) {
         _eventAggregator = eventAggregator;
         _wallContainerController = wallContainerController;
         _sharedWallContext = sharedWallContext;
         CurrentPostFilter = new CompositePostFilter();
         FilteredItems = new RangeObservableCollection<PostControl>();

         _wallContainerController.Container.PullInvoked += onWallContainerPullInvoked;
         _wallContainerController.Container.PullCompleted += onWallContainerPullCompleted;
         _wallContainerController.Container.UploadRequested += onWallContainerUploadRequest;

         FilterPostsIsChecked = true;
         FilterRepostsIsChecked = true;

         PostFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
               updateFilter();
               await filterOutAsync(_wallContainerController.Container.Items).ConfigureAwait(false);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         PostFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
               updateFilter();
               await filterOutAsync(_wallContainerController.Container.Items).ConfigureAwait(false);
            }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_wallContainerController.Container.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         RepostFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_wallContainerController.Container.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);
      }

      private void onWallContainerUploadRequest(object sender, PostControl postControl) {
         _eventAggregator.GetEvent<UploaderEvents.Configure>().Publish(new UploaderViewModelConfiguration() {
            IsEnabled = true,
            DateOverride = postControl.Post.DateUnix
         });
         _eventAggregator.GetEvent<UploaderEvents.SetVisibility>().Publish(true);
      }

      private void onWallContainerPullInvoked(object sender, EventArgs eventArgs) {
         IsBusy = true;
      }

      protected virtual async void onWallContainerPullCompleted(object sender, ObservableCollection<PostControl> items) {
         IsBusy = true;
         try {
            await filterOutAsync(items);
         }
         finally {
            IsBusy = false;
         }
      }

      public virtual void OnNavigatedTo(NavigationContext navigationContext) {
         updateFilter();
      }

      public virtual bool IsNavigationTarget(NavigationContext navigationContext) {
         return !IsBusy;
      }

      public virtual void OnNavigatedFrom(NavigationContext navigationContext) {

      }

      protected async Task filterOutAsync(IEnumerable<PostControl> items) {
         IsBusy = true;
         try {
            var currentFiltered = _filteredItems;

            IEnumerable<PostControl> tempItems = new List<PostControl>();
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
   }
}