using JetBrains.Annotations;
using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models.UrlHelper {
   public interface IImageUrlObtainer {
      [NotNull]
      ImageItem Obtain(Attachment attachment);
   }
}