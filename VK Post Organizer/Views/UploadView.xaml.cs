using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using vk.Models;
using vk.ViewModels;

namespace vk.Views {
   public class UploadInfo {
      [NotNull]
      public WallContainer Wall { get; private set; }
      public int DateOverride { get; private set; }

      [NotNull]
      public IEnumerable<string> Files { get; private set; }

      public UploadInfo([NotNull] WallContainer wall, [CanBeNull] IEnumerable<string> files, int dateOverride = -1) {
         if (wall == null) {
            throw new ArgumentNullException(nameof(wall));
         }

         Wall = wall;
         DateOverride = dateOverride;
         Files = files ?? new List<string>();
      }
   }

   /// <summary>
   /// Interaction logic for UploadWindow.xaml
   /// </summary>
   public partial class UploadView : Window {
      private readonly UploadViewModel _viewModel;

      public static bool IsOpened { get; private set; }
      [CanBeNull]
      public static UploadView OpenedInstance { get; private set; }

      public UploadView() {
         //if (viewModel == null) {
         //   throw new ArgumentNullException(nameof(viewModel));
         //}

         InitializeComponent();

         IsOpened = true;
         OpenedInstance = this;
         //base.DataContext = _viewModel;
         _viewModel = (UploadViewModel)DataContext; //viewModel;
      }


      public async Task ConfigureAsync([NotNull] UploadInfo info) {
         if (info == null) {
            throw new ArgumentNullException(nameof(info));
         }
         await _viewModel.ConfigureAsync(info);
      }

      private void onCloseClick(object sender, RoutedEventArgs e) {
         this.Close();
      }

      protected override void OnClosing(CancelEventArgs e) {
         base.OnClosing(e);
         if (_viewModel.IsBusy) {
            var result = MessageBox.Show("Some work in progress, interrupt?", "App is busy", MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) {
               _viewModel.InterruptAllWork();
            }
            else {
               e.Cancel = true;
            }
         }

      }

      private async void onFilesDrop(object sender, DragEventArgs e) {
         var files = (string[])e.Data.GetData(DataFormats.FileDrop);
         await _viewModel.ImportFilesAsync(files);
         e.Handled = true;
      }

      private void onLoaded(object sender, RoutedEventArgs e) {
      }

      private void onClosed(object sender, EventArgs e) {
         IsOpened = false;
         OpenedInstance = null;
      }
   }
}
