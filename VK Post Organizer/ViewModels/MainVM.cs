using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json.Linq;
using vk.Models;
using vk.Views;

namespace vk.ViewModels {
   class MainVM : BindableBase, IVM {

      private string _content;
      public ICommand ConfigureContentCommand { get; set; }
      public ICommand UploadCommand { get; set; }
      public ICommand AuthorizeCommand { get; set; }

      public string Content {
         get { return _content; }
         set { SetProperty(ref _content, value); }
      }

      public MainVM() {
         ConfigureContentCommand = new DelegateCommand(configureContentCommandExecute);
         UploadCommand = new DelegateCommand(uploadCommandExecute);
         AuthorizeCommand = new DelegateCommand(authorizeCommandExecute);
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

         var method = "https://api.vk.com/method/account.getProfileInfo";
         var parameters = "?first_name,last_name";
         var webClient = new WebClient() {Encoding = System.Text.Encoding.UTF8};
         var downloadedString = webClient.DownloadString(method + parameters + "&access_token=" + token.Token);
         var json = JObject.Parse(downloadedString);

         MessageBox.Show(json.ToString());
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
