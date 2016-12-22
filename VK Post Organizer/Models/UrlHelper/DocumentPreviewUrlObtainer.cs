using System.Linq;
using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models.UrlHelper {
   public class DocumentPreviewUrlObtainer : IImageUrlObtainer {
      public ImageItem Obtain(Attachment attachment, ImageSize preferableImageSize = ImageSize.Any) {
         if (attachment.Document.Preview.Photo == null) {
            return new ImageItem("");
         }

         var original = attachment.Document.Preview.Photo.Sizes.FirstOrDefault(s => s.Type == "o");
         var medium = attachment.Document.Preview.Photo.Sizes.FirstOrDefault(s => s.Type == "m");
         var small = attachment.Document.Preview.Photo.Sizes.FirstOrDefault(s => s.Type == "s");

         if (!string.IsNullOrEmpty(original?.Source) && (preferableImageSize & ImageSize.Large) != 0) {
            return new ImageItem(original.Source);
         }
         if (!string.IsNullOrEmpty(medium?.Source) && (preferableImageSize & ImageSize.Medium) != 0) {
            return new ImageItem(medium.Source);
         }
         if (!string.IsNullOrEmpty(small?.Source) && (preferableImageSize & ImageSize.Small) != 0) {
            return new ImageItem(small.Source);
         }

         return new ImageItem("");
      }
   }
}