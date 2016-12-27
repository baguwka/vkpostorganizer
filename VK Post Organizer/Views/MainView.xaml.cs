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
         MainViewData data;
         if (SaveLoaderHelper.TryLoad("Main Window Data", out data)) {
            MainWindow.Width = data.Width;
            MainWindow.Height = data.Heigth;
         }

      }

      private void onLoaded(object sender, RoutedEventArgs e) {
         getViewModel.OnLoad();
         MainViewData data;
         if (SaveLoaderHelper.TryLoad("Main Window Data", out data)) {
            MainWindow.Left = data.X;
            MainWindow.Top = data.Y;
         }
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
