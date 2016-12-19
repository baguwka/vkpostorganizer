using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models.UrlHelper {
   public class PhotoUrlObtainer : IImageUrlObtainer {
      public ImageItem Obtain(Attachment attachment) {
         if (!string.IsNullOrEmpty(attachment.Photo.Photo1280)) {
            return new ImageItem(attachment.Photo.Photo1280);
         }
         if (!string.IsNullOrEmpty(attachment.Photo.Photo604)) {
            return new ImageItem(attachment.Photo.Photo1280);
         }
         if (!string.IsNullOrEmpty(attachment.Photo.Photo75)) {
            return new ImageItem(attachment.Photo.Photo1280);
         }
         return new ImageItem("");
      }
   }
}