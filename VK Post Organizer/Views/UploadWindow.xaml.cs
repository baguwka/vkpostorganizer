using System;
using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using vk.ViewModels;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for UploadWindow.xaml
   /// </summary>
   public partial class UploadWindow : Window {
      private IVM getViewModel => (IVM)DataContext;


      public UploadWindow([NotNull] IEnumerable<PostControl> slots, [CanBeNull] IEnumerable<string> files) {
         if (slots == null) {
            throw new ArgumentNullException(nameof(slots));
         }

         InitializeComponent();

         var vm = (UploadVM)getViewModel;

         vm.PrepareImages(slots, files);
      }
   }
}
