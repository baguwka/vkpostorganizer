using System.Linq;
using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models.UrlHelper {
   public class DocumentPreviewUrlObtainer : IImageUrlObtainer {
      public ImageItem Obtain(Attachment attachment, ImageSize preferableImageSize = ImageSize.Any) {
         if (attachment.Document.Preview.Photo == null) {
            return new ImageItem("", "");
         }

         var originalPreview = attachment.Document.Preview.Photo.Sizes.FirstOrDefault(s => s.Type == "o");
         var mediumPreview = attachment.Document.Preview.Photo.Sizes.FirstOrDefault(s => s.Type == "m");
         var smallPreview = attachment.Document.Preview.Photo.Sizes.FirstOrDefault(s => s.Type == "s");

         if (!string.IsNullOrEmpty(originalPreview?.Source) && (preferableImageSize & ImageSize.Large) != 0) {
            return new ImageItem(originalPreview.Source, attachment.Document.Url);
         }
         if (!string.IsNullOrEmpty(mediumPreview?.Source) && (preferableImageSize & ImageSize.Medium) != 0) {
            return new ImageItem(mediumPreview.Source, attachment.Document.Url);
         }
         if (!string.IsNullOrEmpty(smallPreview?.Source) && (preferableImageSize & ImageSize.Small) != 0) {
            return new ImageItem(smallPreview.Source, attachment.Document.Url);
         }

         return new ImageItem("", "");
      }
   }
}