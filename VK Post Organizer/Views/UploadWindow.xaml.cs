using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using JetBrains.Annotations;
using vk.ViewModels;

namespace vk.Views {
   public class UploadInfo {
      [NotNull]
      public WallControl Wall { get; private set; }
      public int DateOverride { get; private set; }

      [NotNull]
      public IEnumerable<string> Files { get; private set; }

      public UploadInfo([NotNull] WallControl wall, [CanBeNull] IEnumerable<string> files, int dateOverride = -1) {
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
   public partial class UploadWindow : Window {
      private readonly UploadViewModel _viewModel;

      public UploadWindow() {
         //if (viewModel == null) {
         //   throw new ArgumentNullException(nameof(viewModel));
         //}

         InitializeComponent();

         //base.DataContext = _viewModel;
         _viewModel = (UploadViewModel)DataContext; //viewModel;
      }

      public void Configure([NotNull] UploadInfo info) {
         if (info == null) {
            throw new ArgumentNullException(nameof(info));
         }
         _viewModel.Configure(info);
      }

      private async void UploadWindow_OnDrop(object sender, DragEventArgs e) {
         var files = (string[])e.Data.GetData(DataFormats.FileDrop);
         await _viewModel.ImportFilesAsync(files);
         e.Handled = true;
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
   }
}
