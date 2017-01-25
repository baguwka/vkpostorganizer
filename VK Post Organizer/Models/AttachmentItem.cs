using System;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models.VkApi.Entities;

namespace vk.Models {
   public class AttachmentItem : BindableBase {
      private Photo _photo;

      public string Attachment { get; private set; }

      public event EventHandler RemoveRequested;

      public ICommand OpenInBrowserCommand { get; set; }
      public ICommand RemoveCommand { get; set; }

      public Photo Photo {
         get { return _photo; }
         set { SetProperty(ref _photo, value); }
      }

      public string _preview;

      public AttachmentItem() {
         RemoveCommand = new DelegateCommand(onRemoveRequested);
         OpenInBrowserCommand = new DelegateCommand(openInBrowserExecute);
      } 

      private void openInBrowserExecute() {
         var largestImageUrl = _photo?.GetLargest();
         if (!string.IsNullOrEmpty(largestImageUrl)) {
            Process.Start(largestImageUrl);
         }
      }

      protected virtual void onRemoveRequested() {
         RemoveRequested?.Invoke(this, EventArgs.Empty);
      }

      public void Set(string kind, Photo photo) {
         _photo = photo;
         Attachment = $"{kind}{photo.OwnerId}_{photo.Id}";
      }
   }
}