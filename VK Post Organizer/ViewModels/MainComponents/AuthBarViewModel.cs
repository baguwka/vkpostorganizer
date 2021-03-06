﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using vk.Events;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Views;

namespace vk.ViewModels {
   public class AuthBarViewModel : BindableBase {
      public const string DEFAULT_AVATAR =
         "pack://application:,,,/VKPostOrganizer;component/Resources/default_avatar.png";

      private readonly IEventAggregator _eventAggregator;
      private readonly VkApiProvider _vkApi;
      private bool _isAuthorized;
      private bool _isBusy;
      private ImageSource _profilePhoto;
      private string _userName;

      public bool IsBusy {
         get { return _isBusy; }
         set {
            SetProperty(ref _isBusy, value); 
            _eventAggregator.GetEvent<AuthBarEvents.BusyEvent>().Publish(_isBusy);
         }
      }

      public bool IsAuthorized {
         get { return _isAuthorized; }
         set { SetProperty(ref _isAuthorized, value); }
      }

      public ImageSource ProfilePhoto {
         get { return _profilePhoto; }
         set { SetProperty(ref _profilePhoto, value); }
      }

      public string UserName {
         get { return _userName; }
         set { SetProperty(ref _userName, value); }
      }

      public DelegateCommand AuthorizeCommand { get; private set; }
      public DelegateCommand LogOutCommand { get; private set; }

      public AuthBarViewModel(IEventAggregator eventAggregator, VkApiProvider vkApi) {
         _eventAggregator = eventAggregator;
         _vkApi = vkApi;

         AuthorizeCommand = new DelegateCommand(authorizeCommandExecute, () => !IsBusy).ObservesProperty(() => IsBusy);
         LogOutCommand = new DelegateCommand(logOutCommandExecute, () => !IsBusy).ObservesProperty(() => IsBusy);

         _eventAggregator.GetEvent<AuthBarEvents.AuthorizeIfAlreadyLoggined>().Subscribe(authorizeIfAlreadyLoggined);
         _eventAggregator.GetEvent<VkAuthorizationEvents.AcquiredTheToken>().Subscribe(onTokenAcquired);
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
         var result = MessageBox.Show("Are you sure you want to log out?",
            "Logging out", MessageBoxButton.YesNo);

         if (result == MessageBoxResult.Yes) {
            Deauthorize();
         }
      }

      //todo: деавторизация не работает!
      public void Deauthorize() {
         _eventAggregator.GetEvent<AuthBarEvents.LogOutRequest>().Publish();

         IsAuthorized = false;
         UserName = string.Empty;
         SetUpAvatar(DEFAULT_AVATAR);

         _vkApi.Token.Set(new AccessToken());

         var authWindow = new AuthView();//App.Container.GetInstance<AuthView>();
         authWindow.Action = AuthAction.Deauthorize;
         authWindow.ShowDialog();

         VkAuthorization.ClearAllCookies();
         //App.SuppressWininetBehavior();

         _eventAggregator.GetEvent<AuthBarEvents.LogOutCompleted>().Publish();
      }

      private async void onTokenAcquired(AccessToken accessToken) {
         if (string.IsNullOrEmpty(accessToken.Token)) {
            MessageBox.Show("No access token aquired", "Error while authorization occured", MessageBoxButton.OK,
               MessageBoxImage.Error);
            IsAuthorized = false;
            _eventAggregator.GetEvent<AuthBarEvents.AuthorizationCompleted>().Publish(false);
            return;
         }

         _vkApi.Token.Set(accessToken);

         IsBusy = true;
         try {
            var users = await _vkApi.UsersGet.GetAsync(QueryParameters.New()
               .Add("fields", "first_name,last_name,nickname,photo_50"));

            var user = users.Content.FirstOrDefault();
            if (user == null) {
               MessageBox.Show("Cant find user", "Error while authoriazation occured", MessageBoxButton.OK,
                  MessageBoxImage.Error);
               IsAuthorized = false;
               _eventAggregator.GetEvent<AuthBarEvents.AuthorizationCompleted>().Publish(false);
               return;
            }

            UserName = $"Вы вошли как\n{user.Name}";
            SetUpAvatar(user.Photo50);

            IsAuthorized = true;

            _eventAggregator.GetEvent<AuthBarEvents.AuthorizationCompleted>().Publish(true);

            await _vkApi.StatsTrackVisitor.TrackAsync();
         }
         catch (VkException ex) {
            MessageBox.Show(ex.Message, $"Error while authoriazation occured\n{ex.Message}", MessageBoxButton.OK,
               MessageBoxImage.Error);
            IsAuthorized = false;
         }
         finally {
            IsBusy = false;
         }
      }

      public void Authorize(bool clearCookies) {
         _eventAggregator.GetEvent<AuthBarEvents.AuthorizeRequest>().Publish();
         //var accessToken = App.Container.GetInstance<AccessToken>();
         //var authWindow = new AuthView(accessToken, clearCookies);

         if (clearCookies) {
            VkAuthorization.ClearCookies();
         }

         var authWindow = new AuthView();//App.Container.GetInstance<AuthView>();
         authWindow.Action = AuthAction.Authorize;
         authWindow.ShowDialog();

         //_regionManager.RequestNavigate(RegionNames.MainRegion, $"VkAuthorization?clearcookies={clearCookies}");
      }

      private void authorizeIfAlreadyLoggined() {
         Authorize(false);
         //try {
         //   var cookies = Application.GetCookie(new Uri("https://www.vk.com"));
         //   if (!string.IsNullOrEmpty(cookies)) {
         //      var values = cookies.Split(';');

         //      if (values.Where(s => s.IndexOf('=') > 0).Any(s => s.Substring(0, s.IndexOf('=')).Trim() == "remixsid")) {
         //         Authorize(false);
         //      }
         //   }
         //}
         //catch {
         //   // ignored
         //}
      }
   }

   public enum AuthAction {
      Authorize,
      Deauthorize
   }
}