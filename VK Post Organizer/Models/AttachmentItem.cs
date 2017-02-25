using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using vk.Models.UrlHelper;
using vk.Models.VkApi.Entities;

namespace vk.Models {
   public class AttachmentItem : BindableBase {
      public string Preview { get; private set; }
      public Attachment Attachment { get; private set; }

      public event EventHandler RemoveRequested;

      public ICommand OpenInBrowserCommand { get; private set; }
      public ICommand RemoveCommand { get; private set; }

      public AttachmentItem() {
         Attachment = new Attachment();
         RemoveCommand = new DelegateCommand(onRemoveRequested);
         OpenInBrowserCommand = new DelegateCommand(openInBrowserExecute/*, () => Photo != null).ObservesProperty(() => Photo*/);
      } 

      private void openInBrowserExecute() {
         var largestImageUrl = Attachment.Photo?.GetLargest();
         if (!string.IsNullOrEmpty(largestImageUrl)) {
            Process.Start(largestImageUrl);
         }
      }

      protected virtual void onRemoveRequested() {
         RemoveRequested?.Invoke(this, EventArgs.Empty);
      }

      public void SetAsPhotoAttachment(Photo photo) {
         Attachment.Type = "photo";
         Attachment.Photo = photo;

         Preview = Attachment.Photo.Photo75;
      }

      public void SetAsDocumentAttachment(Document doc) {
         Attachment.Type = "document";
         Attachment.Document = doc;

         var previewObtainer = new DocumentPreviewUrlObtainer();
         Preview = previewObtainer.Obtain(Attachment, ImageSize.Small).Url;
      }
   }

   public static class AttachmentsExtensins {
      public static Attachments ToAttachments(this IEnumerable<AttachmentItem> items) {
         var attachments = new Attachments();
         foreach (var item in items) {
            attachments.Add(item.Attachment);
         }
         return attachments;
      }
   }
}