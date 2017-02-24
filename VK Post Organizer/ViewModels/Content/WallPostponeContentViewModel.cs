using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using vk.Models;
using vk.Models.Filter;

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

      public WallPostponeContentViewModel(IEventAggregator eventAggregator, WallContainerController wallContainerController, SharedWallContext sharedWallContext) : base(eventAggregator, wallContainerController, sharedWallContext) {
         FilterMissingIsChecked = true;

         MissingFilterCheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_wallContainerController.Container.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

         MissingFilterUncheckedCommand = DelegateCommand.FromAsyncHandler(async () => {
            updateFilter();
            await filterOutAsync(_wallContainerController.Container.Items).ConfigureAwait(false);
         }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);
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
            _wallContainerController.Container.WallHolder = _sharedWallContext.SelectedWallHolder;
            await _wallContainerController.Container.PullWithScheduleHightlightAsync(new NoPostFilter(), new Schedule());
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