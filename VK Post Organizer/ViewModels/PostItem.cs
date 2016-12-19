using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models;
using vk.Models.UrlHelper;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   public class PostItem : BindableBase {
      private bool _expanded;
      public Post PostRef { get; }

      public SmartCollection<ImageItem> Images { get; }

      public bool Expanded {
         get { return _expanded; }
         set { SetProperty(ref _expanded, value); }
      }

      public ICommand ExpandToggleCommand { get; set; }

      private void loadImages() {
         foreach (var attachment in PostRef.Attachments) {
            if (attachment.Type == "photo") {
               Images.Add(new PhotoUrlObtainer().Obtain(attachment));
            }

            if (attachment.Type == "doc") {
               if (attachment.Document.Type == (int)DocType.Image || attachment.Document.Type == (int)DocType.Gif) {
                  Images.Add(new DocumentUrlObtainer().Obtain(attachment));
               }
            }
         }
      }

      public PostItem(Post postRef) {
         Images = new SmartCollection<ImageItem>();
         PostRef = postRef;

         ExpandToggleCommand = new DelegateCommand(ExpandToggle);

         var prev = PostRef.CopyHistory?.FirstOrDefault();
         if (prev == null) {
            loadImages();
            return;
         }

         PostRef.ID = prev.ID;
         PostRef.Text = prev.Text;
         PostRef.Attachments = prev.Attachments;

         loadImages();
      }

      private void ExpandToggle() {
         Expanded = !Expanded;
      }

      public void Clear() {
         Images.Clear();
      }

      public void Expand() {
         Expanded = true;
      }

      public void Collapse() {
         Expanded = false;
      }
   }
}