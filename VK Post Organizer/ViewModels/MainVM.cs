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

      public WallList WallList { get; }
      public WallVM Wall { get; }

      private const string DEFAULT_AVATAR =
         "pack://application:,,,/VKPostOrganizer;component/Resources/default_avatar.png";

      public ICommand ConfigureContentCommand { get; set; }
      public ICommand BackCommand { get; set; }
      public ICommand RefreshCommand { get; set; }
      public ICommand TestPostCommand { get; set; }
      public ICommand AuthorizeCommand { get; set; }

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

      public MainVM() {
         ConfigureContentCommand = new DelegateCommand(configureContentCommandExecute);
         BackCommand = new DelegateCommand(backCommandExecute);
         RefreshCommand = new DelegateCommand(refreshCommandExecute);
         AuthorizeCommand = new DelegateCommand(authorizeCommandExecute);
         TestPostCommand = new DelegateCommand(testPostCommandExecute);

         LogOutCommand = new DelegateCommand(logOutCommandExecute);

         WallList = App.Container.Resolve<WallList>();
         Wall = App.Container.Resolve<WallVM>();

         WallList.ItemClicked += onGroupItemClicked;

         CurrentPostTypeFilter = PostType.Both;
      }

      private void testPostCommandExecute() {
         if (TestingGroup != Wall.WallHolder.ID) {
            var groupsGet = App.Container.Resolve<GroupsGetById>();
            var response = groupsGet.Get(TestingGroup, "");
            var group = response.Response.FirstOrDefault();

            MessageBox.Show($"You're only available to post in \"{group?.Name}\" wall in testing purposes.", "Cant post here",
               MessageBoxButton.OK, MessageBoxImage.Error);
            return;
         }

         var wallPost = App.Container.Resolve<WallPost>();

         var unixTimestamp = UnixTimeConverter.ToUnix(new DateTime(2016, 12, 21, 18, 30, 0));

         try {
            var post = wallPost.Post(Wall.WallHolder.ID, "Тест :3", false, true, unixTimestamp);
            refreshCommandExecute();
         }
         catch (VkException ex) {
            MessageBox.Show(ex.Message);
         }
      }

      private void applyFilter(PostType currentPostTypeFilter) {
         if (!IsWallShowing) return;

         Wall?.Pull(currentPostTypeFilter.GetFilter());
      }

      private void onGroupItemClicked(object sender, WallItem wallItem) {
         IsWallShowing = true;
         CanTestPost = TestingGroup == wallItem.WallHolder.ID;

         try {
            Wall.Pull(wallItem.WallHolder, CurrentPostTypeFilter.GetFilter());
         }
         catch (VkException ex) {
            IsWallShowing = false;

            MessageBox.Show(ex.Message, "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      private void configureContentCommandExecute() {
         var configureContentView = new ConfigureContentView();
         configureContentView.ShowDialog();
      }

      private void fillWallList() {
         var methodGroupsGet = App.Container.Resolve<GroupsGet>();
         var groups = methodGroupsGet.Get().Collection;
         if (groups == null) {
            MessageBox.Show("Groups null");
            return;
         }

         WallList.Clear();

         var userget = App.Container.Resolve<UsersGet>();
         var user = userget.Get().Users[0];

         //todo: get rid of workaround
         var item = new WallItem(new EmptyWallHolder {Name = user.Name, Photo200 = user.Photo200, Photo50 = user.Photo50});

         WallList.Add(item);

         foreach (var group in groups.Groups) {
            WallList.Add(new WallItem(group));
         }
      }

      private void backCommandExecute() {
         IsWallShowing = false;
         Wall.Clear();
      }

      private void refreshCommandExecute() {
         Messenger.Broadcast("Refresh");
         if (IsWallShowing) {
            Wall.Pull(CurrentPostTypeFilter.GetFilter());
         }
      }

      private void authorizeIfAlreadyLoggined() {
         var cookies = Application.GetCookie(new Uri("https://www.vk.com"));
         if (!string.IsNullOrEmpty(cookies)) {
            var values = cookies.Split(';');

            if (values.Where(s => s.IndexOf('=') > 0).Any(s => s.Substring(0, s.IndexOf('=')).Trim() == "remixsid")) {
               Authorize();
            }
         }
      }

      public void Authorize() {
         var accessToken = new AccessToken();
         App.Container.RegisterInstance(accessToken);
         var authWindow = new AuthView(accessToken);
         authWindow.ShowDialog();

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
            MessageBox.Show(ex.Message, "Error while authoriazation occured", MessageBoxButton.OK, MessageBoxImage.Error);
            IsAuthorized = false;
         }
      }

      public void Deauthorize() {
         WallList.Clear();
         IsAuthorized = false;
         Content = "";
         SetUpAvatar(DEFAULT_AVATAR);

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
         Authorize();
      }

      public void ImportFiles(IEnumerable<string> files) {
         var sb = new StringBuilder();
         foreach (var file in files) {
            sb.AppendLine(file);
         }
         MessageBox.Show($"Importing\n{sb}");
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
