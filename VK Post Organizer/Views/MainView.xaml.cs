using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace vk.Views {
   public partial class MainView : Window {
      public MainView() {
         InitializeComponent();

         var logo = new BitmapImage();
         logo.BeginInit();
         logo.UriSource = new Uri("pack://application:,,,/VKPostOrganizer;component/Resources/default_avatar.png");
         logo.EndInit();

         //ProfilePhotoSource.Source = logo;
      }
   }
}
