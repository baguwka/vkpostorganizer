﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using vk.Models;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Views;

namespace vk.ViewModels {
   class MainVM : BindableBase, IVM {
      private string _content;
      private ImageSource _profilePhoto;
      private bool _isAuthorized;
      private bool _isGroupSelected;

      public WallList WallList { get; }
      public WallInfo WallInfo { get; }

      private const string DEFAULT_AVATAR =
         "pack://application:,,,/VKPostOrganizer;component/Resources/default_avatar.png";

      public ICommand ConfigureContentCommand { get; set; }
      public ICommand BackCommand { get; set; }
      public ICommand AuthorizeCommand { get; set; }

      public ICommand LogOutCommand { get; set; }

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

      public bool IsGroupSelected {
         get { return _isGroupSelected; }
         set { SetProperty(ref _isGroupSelected, value); }
      }

      public MainVM() {
         ConfigureContentCommand = new DelegateCommand(configureContentCommandExecute);
         BackCommand = new DelegateCommand(backCommandExecute);
         AuthorizeCommand = new DelegateCommand(authorizeCommandExecute);

         LogOutCommand = new DelegateCommand(logOutCommandExecute);

         WallList = App.Container.Resolve<WallList>();
         WallInfo = App.Container.Resolve<WallInfo>();

         WallList.ItemClicked += onGroupItemClicked;
      }

      private void onGroupItemClicked(object sender, WallItem wallItem) {
         IsGroupSelected = true;
         try {
            WallInfo.Load(wallItem.WallHolder);
         }
         catch (VkException ex) {
            IsGroupSelected = false;
            MessageBox.Show(ex.Message, "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      private void configureContentCommandExecute() {
         var configureContentView = new ConfigureContentView();
         configureContentView.ShowDialog();
      }

      private void fillGroupCollection() {
         var methodGroupsGet = App.Container.Resolve<GroupsGet>();
         var groups = methodGroupsGet.Get().Collection;
         if (groups == null) {
            MessageBox.Show("Groups null");
            return;
         }

         WallList.Clear();

         var userget = App.Container.Resolve<UsersGet>();
         var user = userget.Get();
         
         WallList.Add(WallList.InstantiateItem(user.Users[0]));

         foreach (var group in groups.Groups) {
            WallList.Add(WallList.InstantiateItem(group));
         }
      }

      private void backCommandExecute() {
         IsGroupSelected = false;
         WallInfo.Clear();
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

         var methodUsersGet = App.Container.Resolve<UsersGet>();
         var user = methodUsersGet.Get().Users.First();

         Content = $"You logged as\n{user.FirstName} {user.LastName}";
         SetUpAvatar(user.Photo50);

         IsAuthorized = true;

         fillGroupCollection();
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

      private void logOutCommandExecute() {
         var result = MessageBox.Show("Are you sure you want to log out?", "Logging out", MessageBoxButton.YesNo);

         if (result == MessageBoxResult.Yes) {
            Deauthorize();
         }
      }

      public void OnLoad() {
         SetUpAvatar(DEFAULT_AVATAR);

         authorizeIfAlreadyLoggined();
      }

      public void OnClosing() {
         throw new NotImplementedException();
      }

      public void OnClosed() {
         throw new NotImplementedException();
      }
   }
}
