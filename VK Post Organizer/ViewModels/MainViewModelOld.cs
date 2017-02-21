//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Windows;
//using Data_Persistence_Provider;
//using JetBrains.Annotations;
//using Messenger;
//using Prism.Commands;
//using Prism.Events;
//using Prism.Mvvm;
//using Prism.Regions;
//using vk.Events;
//using vk.Infrastructure;
//using vk.Models;
//using vk.Models.VkApi.Entities;
//using vk.Utils;
//using vk.Views;

//namespace vk.ViewModels {
//   [UsedImplicitly]
//   public class MainViewModelOld : BindableBase, IViewModel {
//      private readonly RegionManager _regionManager;
//      private readonly IEventAggregator _eventAggregator;
//      private bool _isAuthorized;
//      private bool _isWallSelected;
//      private PostType _currentPostTypeFilter;
//      private string _infoPanel;
//      private bool _isBusy;
//      private bool _showUploadUi;

//      public WallContainer Wall { get; }

//      public DelegateCommand ConfigureScheduleCommand { get; private set; }
//      public DelegateCommand ApplyScheduleCommand { get; private set; }
//      public DelegateCommand ShowHistoryCommand { get; private set; }
//      public DelegateCommand CloseUploadUiCommand { get; private set; }

//      public IEnumerable<ValueDescription> PostTypes => EnumHelper.GetAllValuesAndDescriptions<PostType>();

//      public PostType CurrentPostTypeFilter {
//         get { return _currentPostTypeFilter; }
//         set {
//            SetProperty(ref _currentPostTypeFilter, value);
//            applyFilter(_currentPostTypeFilter);
//         }
//      }

//      public bool IsAuthorized {
//         get { return _isAuthorized; }
//         set { SetProperty(ref _isAuthorized, value); }
//      }

//      public bool IsWallSelected {
//         get { return _isWallSelected; }
//         set {
//            SetProperty(ref _isWallSelected, value);
//            _eventAggregator.GetEvent<ShellEvents.WallSelectedEvent>().Publish(_isWallSelected);
//         }
//      }

//      public string InfoPanel {
//         get { return _infoPanel; }
//         set { SetProperty(ref _infoPanel, value); }
//      }

//      public bool IsBusy {
//         get { return _isBusy; }
//         set {
//            SetProperty(ref _isBusy, value);
//            _eventAggregator.GetEvent<ShellEvents.BusyEvent>().Publish(_isBusy);
//         }
//      }

//      public MainViewModelOld(RegionManager regionManager, IEventAggregator eventAggregator) {
//         _regionManager = regionManager;
//         _eventAggregator = eventAggregator;

//         _eventAggregator.GetEvent<VkAuthorizationEvents.AcquiredTheToken>().Subscribe((accessToken) => {
//            _regionManager.RequestNavigate(RegionNames.MainRegion, "AvailableWalls");
//         });

//         _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(StartPageView));
//         _regionManager.RegisterViewWithRegion(RegionNames.BottomRegion, typeof(MainBottomView));
//         _regionManager.RegisterViewWithRegion(RegionNames.AuthRegion, typeof(AuthBarView));


//         _eventAggregator.GetEvent<WallSelectorEvents.WallSelected>().Subscribe(onWallItemClicked);

//         ConfigureScheduleCommand = new DelegateCommand(configureScheduleCommandExecute, () => !IsBusy).ObservesProperty(() => IsBusy);

//         _eventAggregator.GetEvent<MainBottomEvents.Back>().Subscribe(onBackRequested);
//         _eventAggregator.GetEvent<MainBottomEvents.Refresh>().Subscribe(onRefreshRequested);
//         _eventAggregator.GetEvent<MainBottomEvents.Upload>().Subscribe(onUploadRequested);
//         _eventAggregator.GetEvent<MainBottomEvents.Settings>().Subscribe(onSettingsRequested);

//         _eventAggregator.GetEvent<AuthBarEvents.AuthorizationCompleted>().Subscribe(onAthorizationCompleted);
//         _eventAggregator.GetEvent<AuthBarEvents.LogOutCompleted>().Subscribe(onLogOutCompleted);

//         ApplyScheduleCommand = new DelegateCommand(applyScheduleCommandExecute);

//         ShowHistoryCommand = new DelegateCommand(showHistoryCommand);
//         CloseUploadUiCommand = new DelegateCommand(() => ShowUploadUi = false);

//         Wall = App.Container.GetInstance<WallContainer>();

//         Wall.UploadRequested += onWallsUploadRequested;

//         Wall.Items.CollectionChanged += (sender, args) => {
//            var totalPostCount = Wall.GetRealPostCount();
//            var repostCount = Wall.GetRepostCount();
//            var postCount = Wall.GetPostOnlyCount();
//            var missingPosts = Wall.GetMissingPostCount();

//            InfoPanel = $"Total: {totalPostCount} / {WallContainer.MAX_POSTPONED}" +
//                        $"\nPosts: {postCount}" +
//                        $"\nReposts: {repostCount}" +
//                        $"\nMissing {missingPosts}";
//         };

//         CurrentPostTypeFilter = PostType.All;

//         CurrentSchedule = new Schedule();

//         AsyncMessenger.AddListener("refresh", refreshWallAsync);
//      }

//      private void onAthorizationCompleted(bool result) {
//         IsAuthorized = result;
//         if (result == true) {
//            _eventAggregator.GetEvent<WallSelectorEvents.FillWallRequest>().Publish();
//         }
//         else {
//            _regionManager.RequestNavigate(RegionNames.MainRegion, "StartPage");
//         }
//      }

//      private void onLogOutCompleted() {
//         IsAuthorized = false;
//         _regionManager.RequestNavigate(RegionNames.MainRegion, "StartPage");
//         IsWallSelected = false;
//      }

//      private void showHistoryCommand() {
//         //var history = App.Container.GetInstance<HistoryView>();
//         //history.Show();
//      }

//      private async void onWallsUploadRequested(object sender, PostControl control) {
//         if (UploadView.IsOpened == false) {
//            var upload = App.Container.GetInstance<UploadView>();
//            await upload.ConfigureAsync(new UploadInfo(new WallContainer(Wall.WallHolder), null, control.Post.DateUnix));
//            upload.Show();
//         }
//         else {
//            UploadView.OpenedInstance?.Activate();
//         }
//      }

//      private void onSettingsRequested() {
//         var settings = new SettingsView();
//         settings.ShowDialog();
//      }

//      private async void applyScheduleCommandExecute() {
//         await Wall.PullWithScheduleHightlightAsync(CurrentPostTypeFilter.GetFilter(), CurrentSchedule);
//      }

//      public Schedule CurrentSchedule { get; set; }


//      public bool ShowUploadUi {
//         get { return _showUploadUi; }
//         set { SetProperty(ref _showUploadUi, value); }
//      }


//      public bool IsUploadAllowed() {
//         //if (TestingGroup != Wall.WallHolder.ID) {
//         //   MessageBox.Show($"You're only available to post in \"{GroupNameCache.GetGroupName(TestingGroup)}\" wall in safety purposes.", "Cant upload here",
//         //      MessageBoxButton.OK, MessageBoxImage.Error);
            
//         //   return false;
//         //}
//         return true;
//      }

//      private async void onUploadRequested() {
//         //var logger = App.Container.GetInstance<IPublishLogger>();
//         //await logger.LogAsync(new WallLogPost {
//         //   AttachmentUrls = new List<string>() {
//         //   },
//         //   OwnerId = 34324324,
//         //   PublishDate = UnixTimeConverter.ToUnix(DateTime.Now),
//         //   PostponedToDate = UnixTimeConverter.ToUnix(new DateTime(2017, 04, 21, 18, 10, 00)),
//         //   WallId = 993943934
//         //});
            
//         //ShowUploadUi = true;
//         //return;

//         //return;w
//         if (IsUploadAllowed() == false) {
//            return;
//         }

//         if (UploadView.IsOpened == false) {
//            var upload = App.Container.GetInstance<UploadView>();
//            await upload.ConfigureAsync(new UploadInfo(new WallContainer(Wall.WallHolder), null));
//            upload.Show();
//         }
//         else {
//            UploadView.OpenedInstance?.Activate();
//         }

//      }

//      private async void applyFilter(PostType currentPostTypeFilter) {
//         if (!IsWallSelected) {
//            return;
//         }

//         await doAsyncShit(Wall.PullWithScheduleHightlightAsync(currentPostTypeFilter.GetFilter(), CurrentSchedule));
//      }

//      private async void onWallItemClicked(WallItem wallItem) {
//         IsWallSelected = true;

//         try {
//            IsBusy = true;
//            await Wall.PullWithScheduleHightlightAsync(wallItem.WallHolder, CurrentPostTypeFilter.GetFilter(),
//                  CurrentSchedule);
//         }
//         catch (VkException ex) {
//            IsWallSelected = false;

//            MessageBox.Show(ex.Message, "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
//         }
//         finally {
//            IsBusy = false;
//         }
//      }

//      private void configureScheduleCommandExecute() {
//         //var configureContentView = new ScheduleWindow();
//         //configureContentView.ShowDialog();
//      }

//      private void onBackRequested() {
//         IsWallSelected = false;
//         Wall.Clear();
//      }

//      private async void onRefreshRequested() {
//         if (!IsAuthorized) return;
//         await refreshWallAsync();
//      }

//      private async Task doAsyncShit(Task shit) {
//         try {
//            IsBusy = true;
//            await shit;
//         }
//         finally {
//            IsBusy = false;
//         }
//      } 

//      private async Task refreshWallAsync() {
//         try {
//            if (IsWallSelected) {
//               await doAsyncShit(Wall.PullWithScheduleHightlightAsync(CurrentPostTypeFilter.GetFilter(), CurrentSchedule));
//            }
//            else {
//               _eventAggregator.GetEvent<WallSelectorEvents.FillWallRequest>().Publish();
//               //await doAsyncShit(fillWallList());
//            }
//         }
//         catch (VkException ex) {
//            if (ex.ErrorCode == 6) {
//               MessageBox.Show(ex.Message, "Too much requests", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//            else {
//               throw;
//            }
//         }
//      }

//      //private async Task fillWallList() {
//      //   var methodGroupsGet = App.Container.GetInstance<GroupsGet>();
//      //   var groups = await methodGroupsGet.GetAsync();
//      //   if (groups.Collection == null) {
//      //      MessageBox.Show("Groups not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//      //      return;
//      //   }

//      //   var userget = App.Container.GetInstance<UsersGet>();
//      //   var users = await userget.GetAsync();
//      //   var user = users.Users.FirstOrDefault();
//      //   if (user == null) {
//      //      MessageBox.Show("User not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//      //      return;
//      //   }

//      //   //todo: get rid of workaround
//      //   var item = new WallItem(new EmptyWallHolder {
//      //      Name = user.Name,
//      //      Description = user.Description,
//      //      Photo200 = user.Photo200,
//      //      Photo50 = user.Photo50
//      //   });

//      //   _wallList.Clear();
//      //   _wallList.Add(item);

//      //   foreach (var group in groups.Collection.Groups) {
//      //      _wallList.Add(new WallItem(group));
//      //   }
//      //}

//      public async void OnLoad() {
//         IsBusy = true;

//         //var mainVmData = await VkPostponeSaveLoader.TryLoadAsync<MainVMSaveInfo>("MainVM");
//         //if (mainVmData.Successful) {
//            //TestingGroup = data.TestingGroup;
//         //}

//         //var loadedSettings = await VkPostponeSaveLoader.TryLoadAsync<Settings>("Settings");
//         var currentSettings = App.Container.GetInstance<Settings>();

//         //if (loadedSettings.Successful) {
//         //   //currentSettings.ApplySettings(loadedSettings.Result);
//         //}
//         //else {
//         //   //defaults
//         //   currentSettings.ApplySettings(new Settings());
//         //}

//         //SetUpAvatar(DEFAULT_AVATAR);

//         //await authorizeIfAlreadyLoggined();
//         _eventAggregator.GetEvent<AuthBarEvents.AuthorizeIfAlreadyLoggined>().Publish();

//         IsBusy = false;

//         //_eventAggregator.GetEvent<FillWallListEvent>().Publish();
//      }

//      public void OnClosing() {
//         var some = App.Container.GetInstance<Settings>();
//         //VkPostponeSaveLoader.Save("MainVM", new MainVMSaveInfo());
//         //VkPostponeSaveLoader.Save("Settings", some);
//      }

//      public void OnClosed() {
//      }
//   }

//   [Serializable]
//   public class MainVMSaveInfo : CommonSaveData {
//      public MainVMSaveInfo() {
//      }
//   }
//}
