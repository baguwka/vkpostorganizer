using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models.UrlHelper {
   public class PhotoUrlObtainer : IImageUrlObtainer {
      public ImageItem Obtain(Attachment attachment, ImageSize preferableImageSize = ImageSize.Any) {
         if (!string.IsNullOrEmpty(attachment.Photo.Photo1280) && (preferableImageSize & ImageSize.Large) != 0) {
            return new ImageItem(attachment.Photo.Photo1280);
         }
         if (!string.IsNullOrEmpty(attachment.Photo.Photo604) && (preferableImageSize & ImageSize.Medium) != 0) {
            return new ImageItem(attachment.Photo.Photo604);
         }
         if (!string.IsNullOrEmpty(attachment.Photo.Photo75) && (preferableImageSize & ImageSize.Small) != 0) {
            return new ImageItem(attachment.Photo.Photo75);
         }
         return new ImageItem("");
      }
   }
}