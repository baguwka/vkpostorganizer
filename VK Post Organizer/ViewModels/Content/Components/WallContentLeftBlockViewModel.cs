using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Events;
using vk.Infrastructure;
using vk.Models;
using vk.Models.Pullers;
using vk.Models.VkApi;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallContentLeftBlockViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;
      private readonly VkApiProvider _vkApi;
      private readonly PullersController _pullersController;
      private readonly BusyObserver _busyObserver;
      private string _description;
      private ImageSource _profilePhoto;
      private bool _contentIsBusy;
      private string _name;
      private string _infoPanel;

      private bool _historyUnreadBadgeVisible;
      private int _historyUnreadPostCount;

      public ICommand ShowActualWallCommand { get; private set; }
      public ICommand ShowPostponeWallCommand { get; private set; }
      public ICommand ShowHistoryCommand { get; private set; }
      public ICommand ExpandAllCommand { get; private set; }
      public ICommand CollapseAllCommand { get; private set; }

      public string Description {
         get { return _description; }
         set { SetProperty(ref _description, value); }
      }

      public ImageSource ProfilePhoto {
         get { return _profilePhoto; }
         set { SetProperty(ref _profilePhoto, value); }
      }

      public string Name {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      public string InfoPanel {
         get { return _infoPanel; }
         set { SetProperty(ref _infoPanel, value); }
      }

      public bool ContentIsBusy {
         get { return _contentIsBusy; }
         private set { SetProperty(ref _contentIsBusy, value); }
      }

      public bool HistoryUnreadBadgeVisible {
         get { return _historyUnreadBadgeVisible; }
         set { SetProperty(ref _historyUnreadBadgeVisible, value); }
      }

      public int HistoryUnreadPostCount {
         get { return _historyUnreadPostCount; }
         set { SetProperty(ref _historyUnreadPostCount, value); }
      }

      public void SetProfilePhoto(string url) {
         var bitmap = new BitmapImage();
         bitmap.BeginInit();
         bitmap.UriSource = new Uri(url, UriKind.Absolute);
         bitmap.EndInit();

         ProfilePhoto = bitmap;
      }

      public WallContentLeftBlockViewModel(IEventAggregator eventAggregator, IRegionManager regionManager,
         VkApiProvider vkApi, PullersController pullersController, BusyObserver busyObserver) {

         SetProfilePhoto(AuthBarViewModel.DEFAULT_AVATAR);
         _eventAggregator = eventAggregator;
         _regionManager = regionManager;
         _vkApi = vkApi;
         _pullersController = pullersController;
         _busyObserver = busyObserver;
         _busyObserver.PropertyChanged += (sender, args) => {
            ContentIsBusy = _busyObserver.ContentIsBusy;
         };

         _pullersController.Postponed.WallHolderChanged += onPostponedWallHolderChanged;
         _pullersController.Postponed.PullCompleted += onPostponedPullCompleted;
         _pullersController.History.PullCompleted += onHistoryPullCompleted;

         ShowActualWallCommand =
            new DelegateCommand(
                  () => {
                     var parameters = new NavigationParameters { { "filter", "howdy" } };
                     _regionManager.RequestNavigate(RegionNames.ContentMainRegion, ViewNames.WallActualContent,
                        parameters);
                  }, () => !ContentIsBusy)
               .ObservesProperty(() => ContentIsBusy);

         ShowPostponeWallCommand =
            new DelegateCommand(
                  () => {
                     _regionManager.RequestNavigate(RegionNames.ContentMainRegion,
                        $"{ViewNames.WallPostponeContent}?filter=sayhello");
                  }, () => !ContentIsBusy)
               .ObservesProperty(() => ContentIsBusy);

         ShowHistoryCommand =
            new DelegateCommand(
                  () => {
                     _regionManager.RequestNavigate(RegionNames.ContentMainRegion,
                        $"{ViewNames.HistoryContent}?filter=sayhello");
                  }, () => !ContentIsBusy)
               .ObservesProperty(() => ContentIsBusy);


         ExpandAllCommand = new DelegateCommand(() => {
            _eventAggregator.GetEvent<ContentEvents.LeftBlockExpandAllRequest>().Publish();
         });
         CollapseAllCommand = new DelegateCommand(() => {
            _eventAggregator.GetEvent<ContentEvents.LeftBlockCollapseAllRequest>().Publish();
         });
      }

      private void onHistoryPullCompleted(object sender, ContentPullerEventArgs e) {
         if (!e.Successful) {
            return;
         }

         HistoryUnreadBadgeVisible = e.Items.Any();
         HistoryUnreadPostCount = e.Items.Count;
      }

      private async void onPostponedWallHolderChanged(object sender, IWallHolder wallHolder) {
         var response = await _vkApi.GroupsGetById.GetAsync(wallHolder.ID);
         var thisGroup = response.Content.FirstOrDefault();
         if (thisGroup == null) return;

         Name = thisGroup.Name;
         Description = thisGroup.Description;
         SetProfilePhoto(thisGroup.Photo200);
      }

      private void onPostponedPullCompleted(object sender, ContentPullerEventArgs e) {
         var puller = sender as ContentPuller;
         if (puller == null) return;

         //todo infopanel info dude

         var totalPostCount = puller.Items.Count;//.GetRealPostCount();
         //var repostCount = puller.GetRepostCount();
         //var postCount = puller.GetPostOnlyCount();
         var missingPosts = MissingFiller.MAX_POSTPONED - puller.Items.Count;//.GetMissingPostCount();

         InfoPanel = $"Всего: {totalPostCount} / {MissingFiller.MAX_POSTPONED}" +
                     //$"\nПостов: {postCount}" +
                     //$"\nРепостов: {repostCount}" +
                     $"\nСлоты для отложек: {missingPosts}";
      }
   }
}