using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using vk.Views;

namespace vk.ViewModels {
   class MainVM : BindableBase, IVM {
      public ICommand ConfigureContentCommand { get; set; }
      public ICommand UploadCommand { get; set; }

      public MainVM() {
         ConfigureContentCommand = new DelegateCommand(configureContentCommandExecute);
         UploadCommand = new DelegateCommand(uploadCommandExecute);

      }

      private void configureContentCommandExecute() {
         var configureContentView = new ConfigureContentView();
         configureContentView.ShowDialog();
      }

      private void uploadCommandExecute() {
         MessageBox.Show("This function is not implemented yet", "FYI", MessageBoxButton.OK, MessageBoxImage.Hand);
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
