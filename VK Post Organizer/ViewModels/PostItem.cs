using System.Linq;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   public class Image : BindableBase {
      private string _url;

      public Image(string url) {
         _url = url;
      }

      public string Url {
         get { return _url; }
         set { SetProperty(ref _url, value); }
      }
   }

   public class PostItem : BindableBase {
      public Post PostRef { get; }

      public SmartCollection<Image> Images { get; }

      private void loadImages() {
         foreach (var attachment in PostRef.Attachments) {
            if (attachment.Type == "photo") {
               Images.Add(new Image(attachment.Photo?.Photo604));
            }

            if (attachment.Type == "doc") {
               if (attachment.Document.Type == (int)DocType.Image || attachment.Document.Type == (int)DocType.Gif) {
                  Images.Add(new Image(attachment.Document?.Url));
               }
            }
         }
      }

      public PostItem(Post postRef) {
         Images = new SmartCollection<Image>();
         PostRef = postRef;

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

      public void Clear() {
         foreach (var image in Images) {
            image.Url = string.Empty;
         }
         Images.Clear();
      }
   }
}