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

      public ICommand OpenInBrowserCommand { get; private set; }
      public ICommand RemoveCommand { get; private set; }

      public Photo Photo {
         get { return _photo; }
         private set { SetProperty(ref _photo, value); }
      }

      public AttachmentItem() {
         RemoveCommand = new DelegateCommand(onRemoveRequested);
         OpenInBrowserCommand = new DelegateCommand(openInBrowserExecute, () => Photo != null);
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
         Photo = photo;
         Attachment = $"{kind}{photo.OwnerId}_{photo.Id}";
      }
   }
}