using System;
using System.ComponentModel;
using System.Windows;
using Data_Persistence_Provider;
using vk.ViewModels;

namespace vk.Views {
   public partial class MainView : Window {
      private IViewModel getViewModel => (IViewModel)DataContext;

      public MainView() {
         InitializeComponent();
      }

      private async void onLoaded(object sender, RoutedEventArgs e) {
         getViewModel.OnLoad();
         var windowData = await SaveLoaderHelper.TryLoadAsync<MainViewData>("Main Window Data");

         if (windowData.Successful) {
            MainWindow.Width = windowData.Result.Width;
            MainWindow.Height = windowData.Result.Heigth;
            MainWindow.Left = windowData.Result.X;
            MainWindow.Top = windowData.Result.Y;
         }
      }

      protected override void OnClosed(EventArgs e) {
         base.OnClosed(e);

         Application.Current.Shutdown();
      }

      private void onClosing(object sender, CancelEventArgs e) {
         getViewModel.OnClosing();
         SaveLoaderHelper.Save("Main Window Data", 
            new MainViewData(MainWindow.Width, MainWindow.Height, MainWindow.Left, MainWindow.Top));
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
