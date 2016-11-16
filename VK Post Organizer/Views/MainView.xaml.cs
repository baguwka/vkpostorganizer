using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace vk.Views {
   public partial class MainView : Window {
      public MainView() {
         InitializeComponent();
         //using (MemoryStream ms = new MemoryStream()) {
         //   Properties.Resources.default_avatar.Save(ms, ImageFormat.Png);
         //   ms.Position = 0;
         //   BitmapImage bi = new BitmapImage();
         //   bi.BeginInit();
         //   bi.StreamSource = ms;
         //   bi.EndInit();

         //   ProfilePhotoSource.Source = bi;
         //}

         //ProfilePhotoSource.Source = logo;

      }
   }
}
