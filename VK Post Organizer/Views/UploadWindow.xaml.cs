using System;
using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using vk.ViewModels;

namespace vk.Views {
   public class UploadInfo {
      [NotNull]
      public WallControl Wall { get; private set; }

      [NotNull]
      public IEnumerable<string> Files { get; private set; }

      public UploadInfo([NotNull] WallControl wall, [CanBeNull] IEnumerable<string> files) {
         if (wall == null) {
            throw new ArgumentNullException(nameof(wall));
         }

         Wall = wall;
         Files = files ?? new List<string>();
      }
   }

   /// <summary>
   /// Interaction logic for UploadWindow.xaml
   /// </summary>
   public partial class UploadWindow : Window {
      private IViewModel getViewModel => (IViewModel)DataContext;

      public UploadWindow([NotNull] UploadInfo info) {
         if (info == null) {
            throw new ArgumentNullException(nameof(info));
         }

         InitializeComponent();

         var vm = (UploadViewModel)getViewModel;
         vm.PrepareImages(info);
      }

      private void UploadWindow_OnDrop(object sender, DragEventArgs e) {
         var files = (string[])e.Data.GetData(DataFormats.FileDrop);
         ((UploadViewModel)getViewModel).ImportFiles(files);
         e.Handled = true;
      }

      private void onCloseClick(object sender, RoutedEventArgs e) {
         this.Close();
      }

      private void onFilesDrop(object sender, DragEventArgs e) {
         var files = (string[])e.Data.GetData(DataFormats.FileDrop);
         ((UploadViewModel)getViewModel).ImportFiles(files);
         e.Handled = true;
      }
   }
}
