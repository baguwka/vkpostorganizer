using System.Collections.Generic;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace vk.ViewModels {
   public class UploadVM : BindableBase, IVM, IDropTarget {
      public ICommand UploadCommand { get; set; }

      public void PrepareImages(IEnumerable<PostControl> slots, IEnumerable<string> files) {
         
      }

      public UploadVM() {
         UploadCommand = new DelegateCommand(() => {
            
         });
      }

      public void OnLoad() {
      }

      public void OnClosing() {
      }

      public void OnClosed() {
      }

      public void DragOver(IDropInfo dropInfo) {
      }

      public void Drop(IDropInfo dropInfo) {
      }
   }
}
