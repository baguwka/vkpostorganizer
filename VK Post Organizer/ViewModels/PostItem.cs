using System.Drawing;
using System.Linq;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models.VkApi.Entities;

namespace vk.Models {
   public class PostItem : BindableBase {
      public Post PostRef { get; }
      public string Photo { get; }

      public PostItem(Post postRef) {
         PostRef = postRef;

         Photo = PostRef.Attachments?.FirstOrDefault(a => a.Type == "photo")?.Photo?.Photo604;
         Photo = PostRef.Attachments?.FirstOrDefault(a => a.Type == "doc")?.Photo?.Photo604;

         var prev = PostRef.CopyHistory?.FirstOrDefault();
         if (prev == null) return;
         PostRef.ID = prev.ID;
         PostRef.Text = prev.Text;
         PostRef.Attachments = prev.Attachments;
      }
   }
}