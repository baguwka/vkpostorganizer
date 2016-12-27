using System;
using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using vk.ViewModels;

namespace vk.Views {
   public class UploadInfo {
      [NotNull]
      public IEnumerable<PostControl> Slots { get; private set; }

      [NotNull]
      public IEnumerable<string> Files { get; private set; }

      public int WallID { get; private set; }

      public UploadInfo([NotNull] IEnumerable<PostControl> slots, [CanBeNull] IEnumerable<string> files, int wallID) {
         if (slots == null) {
            throw new ArgumentNullException(nameof(slots));
         }

         Slots = slots;
         Files = files ?? new List<string>();
         WallID = wallID;
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
   }
}
