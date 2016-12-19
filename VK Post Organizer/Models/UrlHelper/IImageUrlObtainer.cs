using System;
using JetBrains.Annotations;
using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models.UrlHelper {
   public interface IImageUrlObtainer {
      [NotNull]
      ImageItem Obtain(Attachment attachment, ImageSize preferableImageSize = ImageSize.Any);
   }

   [Flags]
   public enum ImageSize {
      Large = 1,
      Medium = 2,
      Small = 4,
      Any = Large | Medium | Small
   }
}