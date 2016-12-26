using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Data_Persistence_Provider;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using Utilities;
using vk.Models;
using vk.Models.Files;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Utils;
using vk.Views;

namespace vk.ViewModels {
   class MainVM : BindableBase, IVM {
      private string _content;
      private ImageSource _profilePhoto;
      private bool _isAuthorized;
      private bool _isWallShowing;
      private PostType _currentPostTypeFilter;
      private bool _canTestPost;
      private int _testingGroup;
      private int _missingPosts;
      private int _postCount;
      private int _repostCount;
      private int _totalPostCount;
      private string _infoPanel;

      public WallList ListOfAvaliableWalls { get; }
      public WallVM Wall { get; }

      private const string DEFAULT_AVATAR =
         "pack://application:,,,/VKPostOrganizer;component/Resources/default_avatar.png";

      public ICommand ConfigureScheduleCommand { get; set; }
      public ICommand BackCommand { get; set; }
      public ICommand RefreshCommand { get; set; }
      public ICommand UploadCommand { get; set; }
      public ICommand AuthorizeCommand { get; set; }
      public ICommand ApplyScheduleCommand { get; set; }

      public ICommand LogOutCommand { get; set; }

      public IEnumerable<ValueDescription> PostTypes => EnumHelper.GetAllValuesAndDescriptions<PostType>();

      public PostType CurrentPostTypeFilter {
         get { return _currentPostTypeFilter; }
         set {
            SetProperty(ref _currentPostTypeFilter, value);
            applyFilter(_currentPostTypeFilter);
         }
      }

      public int TestingGroup {
         get { return _testingGroup; }
         set { SetProperty(ref _testingGroup, value); }
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

      public int TotalPostCount {
         get { return _totalPostCount; }
         set { SetProperty(ref _totalPostCount, value); }
      }

      public int RepostCount {
         get { return _repostCount; }
         set { SetProperty(ref _repostCount, value); }
      }

      public int PostCount {
         get { return _postCount; }
         set { SetProperty(ref _postCount, value); }
      }

      public int MissingPosts {
         get { return _missingPosts; }
         set { SetProperty(ref _missingPosts, value); }
      }

      public string InfoPanel {
         get { return _infoPanel; }
         set { SetProperty(ref _infoPanel, value); }
      }


      public MainVM() {
         ConfigureScheduleCommand = new DelegateCommand(configureScheduleCommandExecute);
         BackCommand = new DelegateCommand(backCommandExecute);
         RefreshCommand = new DelegateCommand(refreshCommandExecute);
         AuthorizeCommand = new DelegateCommand(authorizeCommandExecute);
         UploadCommand = new DelegateCommand(uploadCommandExecute);

         LogOutCommand = new DelegateCommand(logOutCommandExecute);

         ApplyScheduleCommand = new DelegateCommand(applyScheduleCommandExecute);

         ListOfAvaliableWalls = App.Container.Resolve<WallList>();
         Wall = App.Container.Resolve<WallVM>();

         Wall.Items.CollectionChanged += (sender, args) => {
            var realPosts = Wall.Items.Where(post => post.IsExisting).ToList();
            TotalPostCount = realPosts.Count();
            RepostCount = realPosts.Count(post => post.PostType == PostType.Repost);
            PostCount = realPosts.Count(post => post.PostType == PostType.Post);
            MissingPosts = Wall.Items.Count(post => post.PostType == PostType.Missing);

            InfoPanel = $"Total: {TotalPostCount} ({TotalPostCount + MissingPosts})\nPosts: {PostCount}\nReposts: {RepostCount}\nMissing {MissingPosts}";
         };

         ListOfAvaliableWalls.ItemClicked += onGroupItemClicked;

         CurrentPostTypeFilter = PostType.All;

         CurrentSchedule = new Schedule();
      }

      private void applyScheduleCommandExecute() {
         Wall?.PullWithScheduleHightlight(CurrentPostTypeFilter.GetFilter(), CurrentSchedule);
      }


      public Schedule CurrentSchedule { get; set; }


      public bool IsUploadAllowed() {
         if (TestingGroup != Wall.WallHolder.ID) {
            MessageBox.Show($"You're only available to post in \"{GroupNameCache.GetGroupName(TestingGroup)}\" wall in safety purposes.", "Cant upload here",
               MessageBoxButton.OK, MessageBoxImage.Error);
            
            return false;
         }
         return true;
      }

      private void uploadCommandExecute() {
         if (IsUploadAllowed() == false) {
            return;
         }

         var upload = new UploadWindow(new UploadInfo(Wall.Items, null, Wall.WallHolder.ID));
         upload.ShowDialog();
      }

      private void applyFilter(PostType currentPostTypeFilter) {
         if (!IsWallShowing) return;

         Wall?.PullWithScheduleHightlight(currentPostTypeFilter.GetFilter(), CurrentSchedule);
      }

      private void onGroupItemClicked(object sender, WallItem wallItem) {
         IsWallShowing = true;
         CanTestPost = TestingGroup == wallItem.WallHolder.ID;

         try {
            Wall.PullWithScheduleHightlight(wallItem.WallHolder, CurrentPostTypeFilter.GetFilter(), CurrentSchedule);
         }
         catch (VkException ex) {
            IsWallShowing = false;

            MessageBox.Show(ex.Message, "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      private void configureScheduleCommandExecute() {
         var configureContentView = new ScheduleWindow();
         configureContentView.ShowDialog();
      }

      private void fillWallList() {
         if (!IsAuthorized) return;

         var methodGroupsGet = App.Container.Resolve<GroupsGet>();
         var groups = methodGroupsGet.Get().Collection;
         if (groups == null) {
            MessageBox.Show("Groups null");
            return;
         }

         ListOfAvaliableWalls.Clear();

         var userget = App.Container.Resolve<UsersGet>();
         var user = userget.Get().Users[0];

         //todo: get rid of workaround
         var item = new WallItem(new EmptyWallHolder {
            Name = user.Name, Description = user.Description, Photo200 = user.Photo200, Photo50 = user.Photo50
         });

         ListOfAvaliableWalls.Add(item);

         foreach (var group in groups.Groups) {
            ListOfAvaliableWalls.Add(new WallItem(group));
         }
      }

      private void backCommandExecute() {
         IsWallShowing = false;
         Wall.Clear();
      }

      private void refreshCommandExecute() {
         if (!IsAuthorized) return;
         Messenger.Broadcast("Refresh");
         if (IsWallShowing) {
            Wall.PullWithScheduleHightlight(CurrentPostTypeFilter.GetFilter(), CurrentSchedule);
         }
         else {
            fillWallList();
         }
      }

      private void authorizeIfAlreadyLoggined() {
         try {
            var cookies = Application.GetCookie(new Uri("https://www.vk.com"));
            if (!string.IsNullOrEmpty(cookies)) {
               var values = cookies.Split(';');

               if (values.Where(s => s.IndexOf('=') > 0).Any(s => s.Substring(0, s.IndexOf('=')).Trim() == "remixsid")) {
                  Authorize(false);
               }
            }
         }
         catch (Exception ex) {
            MessageBox.Show(ex.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      public void Authorize(bool clearCookies) {
         var accessToken = new AccessToken();
         App.Container.RegisterInstance(accessToken);
         var authWindow = new AuthView(accessToken, clearCookies);
         authWindow.ShowDialog();

         if (string.IsNullOrEmpty(accessToken.Token)) {
            MessageBox.Show("No access token aquired", "Error while authorization occured", MessageBoxButton.OK,
               MessageBoxImage.Error);
            IsAuthorized = false;
            return;
         }

         try {
            var methodUsersGet = App.Container.Resolve<UsersGet>();
            var users = methodUsersGet.Get();

            var user = users.Users.FirstOrDefault();
            if (user == null) {
               MessageBox.Show("Cant find user", "Error while authoriazation occured", MessageBoxButton.OK, MessageBoxImage.Error);
               IsAuthorized = false;
               return;
            }

            Content = $"You logged as\n{user.FirstName} {user.LastName}";
            SetUpAvatar(user.Photo50);

            IsAuthorized = true;

            fillWallList();
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

         var accessToken = new AccessToken();
         App.Container.RegisterInstance(accessToken);

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

      private void authorizeCommandExecute() {
         Authorize(true);
      }

      private void logOutCommandExecute() {
         var result = MessageBox.Show("Are you sure you want to log out?", "Logging out", MessageBoxButton.YesNo);

         if (result == MessageBoxResult.Yes) {
            Deauthorize();
         }
      }

      public void OnLoad() {
         MainVMSaveInfo data;
         if (SaveLoaderHelper.TryLoad("MainVM", out data)) {
            TestingGroup = data.TestingGroup;
         }

         SetUpAvatar(DEFAULT_AVATAR);

         authorizeIfAlreadyLoggined();
      }

      public void OnClosing() {
         SaveLoaderHelper.Save("MainVM", new MainVMSaveInfo(TestingGroup));
      }

      public void OnClosed() {
      }
   }

   [Serializable]
   public class MainVMSaveInfo : CommonSaveData {
      public MainVMSaveInfo(int testingGroup) {
         TestingGroup = testingGroup;
      }

      public int TestingGroup { get; set; }
   }
}
