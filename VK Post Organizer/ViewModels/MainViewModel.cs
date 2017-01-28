using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Data_Persistence_Provider;
using Messenger;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Utilities;
using vk.Models;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Utils;
using vk.Views;

namespace vk.ViewModels {
   class MainViewModel : BindableBase, IViewModel {
      private string _content;
      private ImageSource _profilePhoto;
      private bool _isAuthorized;
      private bool _isWallShowing;
      private PostType _currentPostTypeFilter;
      private bool _canTestPost;
      private string _infoPanel;
      private bool _isBusy;

      public WallList ListOfAvaliableWalls { get; }
      public WallControl Wall { get; }

      private const string DEFAULT_AVATAR =
         "pack://application:,,,/VKPostOrganizer;component/Resources/default_avatar.png";


      public DelegateCommand ConfigureScheduleCommand { get; private set; }
      public DelegateCommand BackCommand { get; private set; }
      public DelegateCommand RefreshCommand { get; private set; }
      public DelegateCommand UploadCommand { get; private set; }
      public DelegateCommand AuthorizeCommand { get; private set; }
      public DelegateCommand ApplyScheduleCommand { get; private set; }
      public DelegateCommand SettingsCommand { get; private set; }
      public DelegateCommand LogOutCommand { get; private set; }

      public IEnumerable<ValueDescription> PostTypes => EnumHelper.GetAllValuesAndDescriptions<PostType>();

      public PostType CurrentPostTypeFilter {
         get { return _currentPostTypeFilter; }
         set {
            SetProperty(ref _currentPostTypeFilter, value);
            applyFilter(_currentPostTypeFilter);
         }
      }

      public bool CanTestPost {
         get { return _canTestPost; }
         set { SetProperty(ref _canTestPost, value); }
      }

      public string Content {
         get { return _content; }
         set { SetProperty(ref _content, value); }
      }

      public ImageSource ProfilePhoto {
         get { return _profilePhoto; }
         set { SetProperty(ref _profilePhoto, value); }
      }

      public bool IsAuthorized {
         get { return _isAuthorized; }
         set { SetProperty(ref _isAuthorized, value); }
      }

      public bool IsWallShowing {
         get { return _isWallShowing; }
         set {
            SetProperty(ref _isWallShowing, value);
            if (value == false) {
               CanTestPost = false;
            }
         }
      }

      public string InfoPanel {
         get { return _infoPanel; }
         set { SetProperty(ref _infoPanel, value); }
      }

      public bool IsBusy {
         get { return _isBusy; }
         set {
            SetProperty(ref _isBusy, value);
            _commandsAffectedByBusyStatus.ForEach(command => command.RaiseCanExecuteChanged());
         }
      }

      private readonly IEnumerable<DelegateCommand> _commandsAffectedByBusyStatus;

      public MainViewModel() {
         ConfigureScheduleCommand = new DelegateCommand(configureScheduleCommandExecute, () => !IsBusy);
         BackCommand = new DelegateCommand(backCommandExecute, () => !IsBusy);
         RefreshCommand = new DelegateCommand(refreshCommandExecute, () => !IsBusy);
         AuthorizeCommand = new DelegateCommand(authorizeCommandExecute, () => !IsBusy);
         UploadCommand = new DelegateCommand(uploadCommandExecute);

         LogOutCommand = new DelegateCommand(logOutCommandExecute, () => !IsBusy);

         ApplyScheduleCommand = new DelegateCommand(applyScheduleCommandExecute);
         SettingsCommand = new DelegateCommand(settingsCommandExecute);

         _commandsAffectedByBusyStatus = new List<DelegateCommand>() {
            BackCommand,ConfigureScheduleCommand, RefreshCommand, AuthorizeCommand, LogOutCommand
         };

         ListOfAvaliableWalls = App.Container.GetInstance<WallList>();
         Wall = App.Container.GetInstance<WallControl>();

         Wall.UploadRequested += onWallsUploadRequested;

         Wall.Items.CollectionChanged += (sender, args) => {
            var totalPostCount = Wall.GetRealPostCount();
            var repostCount = Wall.GetRepostCount();
            var postCount = Wall.GetPostOnlyCount();
            var missingPosts = Wall.GetMissingPostCount();

            InfoPanel = $"Total: {totalPostCount} / {WallControl.MAX_POSTPONED}" +
                        $"\nPosts: {postCount}" +
                        $"\nReposts: {repostCount}" +
                        $"\nMissing {missingPosts}";
         };

         ListOfAvaliableWalls.ItemClicked += onGroupItemClicked;

         CurrentPostTypeFilter = PostType.All;

         CurrentSchedule = new Schedule();

         AsyncMessenger.AddListener("refresh", refreshWallAsync);
      }

      private async void onWallsUploadRequested(object sender, PostControl control) {
         var upload = App.Container.GetInstance<UploadView>();
         await upload.ConfigureAsync(new UploadInfo(new WallControl(Wall.WallHolder), null, control.Post.DateUnix));
         upload.Show();
      }

      private void settingsCommandExecute() {
         var settings = new SettingsView();
         settings.ShowDialog();
      }

      private async void applyScheduleCommandExecute() {
         await Wall.PullWithScheduleHightlightAsync(CurrentPostTypeFilter.GetFilter(), CurrentSchedule);
      }


      public Schedule CurrentSchedule { get; set; }

      public bool IsUploadAllowed() {
         //if (TestingGroup != Wall.WallHolder.ID) {
         //   MessageBox.Show($"You're only available to post in \"{GroupNameCache.GetGroupName(TestingGroup)}\" wall in safety purposes.", "Cant upload here",
         //      MessageBoxButton.OK, MessageBoxImage.Error);
            
         //   return false;
         //}
         return true;
      }

      private async void uploadCommandExecute() {
         if (IsUploadAllowed() == false) {
            return;
         }

         var upload = App.Container.GetInstance<UploadView>();
         await upload.ConfigureAsync(new UploadInfo(new WallControl(Wall.WallHolder), null));
         upload.Show();
      }

      private async void applyFilter(PostType currentPostTypeFilter) {
         if (!IsWallShowing) {
            return;
         }

         await doAsyncShit(Wall.PullWithScheduleHightlightAsync(currentPostTypeFilter.GetFilter(), CurrentSchedule));
      }

      private async void onGroupItemClicked(object sender, WallItem wallItem) {
         IsWallShowing = true;

         try {
            IsBusy = true;
            await Wall.PullWithScheduleHightlightAsync(wallItem.WallHolder, CurrentPostTypeFilter.GetFilter(),
                  CurrentSchedule);
         }
         catch (VkException ex) {
            IsWallShowing = false;

            MessageBox.Show(ex.Message, "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
         }
         finally {
            IsBusy = false;
         }
      }

      private void configureScheduleCommandExecute() {
         //var configureContentView = new ScheduleWindow();
         //configureContentView.ShowDialog();
      }

      private async Task fillWallList() {
         if (!IsAuthorized) return;

         var methodGroupsGet = App.Container.GetInstance<GroupsGet>();
         var groups = await methodGroupsGet.GetAsync();
         if (groups.Collection == null) {
            MessageBox.Show("Groups not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
         }

         var userget = App.Container.GetInstance<UsersGet>();
         var users = await userget.GetAsync();
         var user = users.Users.FirstOrDefault();
         if (user == null) {
            MessageBox.Show("User not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
         }

         //todo: get rid of workaround
         var item = new WallItem(new EmptyWallHolder {
            Name = user.Name, Description = user.Description, Photo200 = user.Photo200, Photo50 = user.Photo50
         });

         ListOfAvaliableWalls.Clear();
         ListOfAvaliableWalls.Add(item);

         foreach (var group in groups.Collection.Groups) {
            ListOfAvaliableWalls.Add(new WallItem(group));
         }
      }

      private void backCommandExecute() {
         IsWallShowing = false;
         Wall.Clear();
      }

      private async void refreshCommandExecute() {
         if (!IsAuthorized) return;
         await refreshWallAsync();
      }

      private async Task doAsyncShit(Task shit) {
         try {
            IsBusy = true;
            await shit;
         }
         finally {
            IsBusy = false;
         }
      } 

      private async Task refreshWallAsync() {
         try {
            if (IsWallShowing) {
               await doAsyncShit(Wall.PullWithScheduleHightlightAsync(CurrentPostTypeFilter.GetFilter(), CurrentSchedule));
            }
            else {
               await doAsyncShit(fillWallList());
            }
         }
         catch (VkException ex) {
            if (ex.ErrorCode == 6) {
               MessageBox.Show(ex.Message, "Too much requests", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else {
               throw;
            }
         }
      }

      private async Task authorizeIfAlreadyLoggined() {
         try {
            var cookies = Application.GetCookie(new Uri("https://www.vk.com"));
            if (!string.IsNullOrEmpty(cookies)) {
               var values = cookies.Split(';');

               if (values.Where(s => s.IndexOf('=') > 0).Any(s => s.Substring(0, s.IndexOf('=')).Trim() == "remixsid")) {
                  await Authorize(false);
               }
            }
         }
         catch (Exception ex) {
            MessageBox.Show(ex.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      public async Task Authorize(bool clearCookies) {
         var accessToken = App.Container.GetInstance<AccessToken>();
         var authWindow = new AuthView(accessToken, clearCookies);

         authWindow.ShowDialog();

         if (string.IsNullOrEmpty(accessToken.Token)) {
            MessageBox.Show("No access token aquired", "Error while authorization occured", MessageBoxButton.OK,
               MessageBoxImage.Error);
            IsAuthorized = false;
            return;
         }

         try {
            var methodUsersGet = App.Container.GetInstance<UsersGet>();
            var users = await methodUsersGet.GetAsync();

            var user = users.Users.FirstOrDefault();
            if (user == null) {
               MessageBox.Show("Cant find user", "Error while authoriazation occured", MessageBoxButton.OK, MessageBoxImage.Error);
               IsAuthorized = false;
               return;
            }

            Content = $"You logged as\n{user.FirstName} {user.LastName}";
            SetUpAvatar(user.Photo50);

            IsAuthorized = true;

            await App.Container.GetInstance<StatsTrackVisitor>().TrackAsync();

            await fillWallList();
         }
         catch (VkException ex) {
            MessageBox.Show(ex.Message, $"Error while authoriazation occured\n{ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
            IsAuthorized = false;
         }
      }

      public void Deauthorize() {
         IsWallShowing = false;
         ListOfAvaliableWalls.Clear();
         IsAuthorized = false;
         Content = "";
         SetUpAvatar(DEFAULT_AVATAR);

         var accessToken = App.Container.GetInstance<AccessToken>();
         accessToken.Set(new AccessToken());

         var expiration = DateTime.UtcNow - TimeSpan.FromDays(1);
         string cookie = $"remixsid=; expires={expiration.ToString("R")}; path=/; domain=.vk.com";
         Application.SetCookie(new Uri("https://www.vk.com"), cookie);
      }


      public void SetUpAvatar(string url) {
         var bitmap = new BitmapImage();
         bitmap.BeginInit();
         bitmap.UriSource = new Uri(url, UriKind.Absolute);
         bitmap.EndInit();

         ProfilePhoto = bitmap;
      }

      private async void authorizeCommandExecute() {
         await Authorize(true);
      }

      private void logOutCommandExecute() {
         var result = MessageBox.Show("Are you sure you want to log out?", 
            "Logging out", MessageBoxButton.YesNo);

         if (result == MessageBoxResult.Yes) {
            Deauthorize();
         }
      }

      public async void OnLoad() {
         IsBusy = true;

         var mainVmData = await SaveLoaderHelper.TryLoadAsync<MainVMSaveInfo>("MainVM");
         if (mainVmData.Successful) {
            //TestingGroup = data.TestingGroup;
         }

         var loadedSettings = await SaveLoaderHelper.TryLoadAsync<Settings>("Settings");
         var currentSettings = App.Container.GetInstance<Settings>();

         if (loadedSettings.Successful) {
            currentSettings.ApplySettings(loadedSettings.Result);
         }
         else {
            //defaults
            currentSettings.ApplySettings(new Settings() {
               Upload = new UploadSettings {
                  CloseUploadWindowAfterPublish = true,
                  SignedPosting = false,
                  PostFromGroup = true
               },
               Proxy = new ProxySettings {
                  UseProxy = false
               }
            });
         }

         SetUpAvatar(DEFAULT_AVATAR);

         await authorizeIfAlreadyLoggined();

         IsBusy = false;
      }

      public void OnClosing() {
         var some = App.Container.GetInstance<Settings>();
         SaveLoaderHelper.Save("MainVM", new MainVMSaveInfo());
         SaveLoaderHelper.Save("Settings", some);
      }

      public void OnClosed() {
      }
   }

   [Serializable]
   public class MainVMSaveInfo : CommonSaveData {
      public MainVMSaveInfo() {
      }
      
   }
}
