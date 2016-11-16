using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models;
using vk.Views;

namespace vk.ViewModels {

   class MainVM : BindableBase, IVM {

      private string _content;
      private ImageSource _profilePhoto;
      private bool _isAuthorized;

      public ICommand ConfigureContentCommand { get; set; }
      public ICommand UploadCommand { get; set; }
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

      public MainVM() {
         ConfigureContentCommand = new DelegateCommand(configureContentCommandExecute);
         UploadCommand = new DelegateCommand(uploadCommandExecute);
         AuthorizeCommand = new DelegateCommand(authorizeCommandExecute);

         LogOutCommand = new DelegateCommand(logOutCommandExecute);
      }

      private void configureContentCommandExecute() {
         var configureContentView = new ConfigureContentView();
         configureContentView.ShowDialog();
      }

      private void uploadCommandExecute() {
         MessageBox.Show("This function is not implemented yet", "FYI", MessageBoxButton.OK, MessageBoxImage.Hand);
      }

      private void authorizeCommandExecute() {
         var token = new AccessToken();
         var authWindow = new AuthView(token);
         authWindow.ShowDialog();

         var methodUsersGet = new VkApiUsersGet(token.Token);
         var user = methodUsersGet.Get().Users.First();

         Content = $"{user.FirstName} {user.LastName}";
         SetUpAvatar(user.UserPhotoUri);

         IsAuthorized = true;
      }


      public void SetUpAvatar(string url) {
         var bitmap = new BitmapImage();
         bitmap.BeginInit();
         bitmap.UriSource = new Uri(url, UriKind.Absolute);
         bitmap.EndInit();

         ProfilePhoto = bitmap;
      }


      private void logOutCommandExecute() {
         MessageBox.Show("Let's pretend we're loggining out ;)");

         IsAuthorized = false;
      }

      public void OnLoad() {
         throw new NotImplementedException();
      }

      public void OnClosing() {
         throw new NotImplementedException();
      }

      public void OnClosed() {
         throw new NotImplementedException();
      }
   }
}
