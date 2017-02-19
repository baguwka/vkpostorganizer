using System;
using System.ComponentModel;
using System.Windows;
using Data_Persistence_Provider;
using vk.ViewModels;

namespace vk.Views {
   public partial class Shell : Window {
      private readonly VkPostponeSaveLoader _saveLoader;
      private readonly ShellViewModel _viewModel;

      public Shell(VkPostponeSaveLoader saveLoader) {
         _saveLoader = saveLoader;
         InitializeComponent();
         _viewModel = (ShellViewModel)DataContext;
      }

      private async void onLoaded(object sender, RoutedEventArgs e) {
         _viewModel.OnLoad();
         var windowData = await _saveLoader.TryLoadAsync<MainViewData>("Main Window Data");

         if (windowData.Successful) {
            ShellWindow.Width = windowData.Result.Width;
            ShellWindow.Height = windowData.Result.Heigth;
            ShellWindow.Left = windowData.Result.X;
            ShellWindow.Top = windowData.Result.Y;
         }
      }

      protected override void OnClosed(EventArgs e) {
         base.OnClosed(e);

         Application.Current.Shutdown();
      }

      private void onClosing(object sender, CancelEventArgs e) {
         _viewModel.OnClosing();
         _saveLoader.Save("Main Window Data", 
            new MainViewData(ShellWindow.Width, ShellWindow.Height, ShellWindow.Left, ShellWindow.Top));
      }

      private void onInitialized(object sender, EventArgs e) {
      }
   }

   [Serializable]
   public class MainViewData : CommonSaveData {
      public MainViewData(double width, double heigth, double x, double y) {
         Width = width;
         Heigth = heigth;
         X = x;
         Y = y;
      }

      public double Width { get; }
      public double Heigth { get; }
      public double X { get; }
      public double Y { get; }
   }
}
