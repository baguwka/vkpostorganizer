using System.Collections.Generic;
using JetBrains.Annotations;

namespace vk.Models.Files {
   [UsedImplicitly]
   public class ImageExtensionChecker : ExtensionChecker {
      public override IEnumerable<string> GetValidExtensions() {
         return new[] { ".jpg", ".jpeg", ".bmp", ".png" };
      }

      public override string GetFileFilter() {
         return "Images (*.jpg, *.jpeg, *.bmp, *.png)|*.jpg;*.jpeg;*.bmp;*.png|All files (*.*)|*.*";
      }
   }
}