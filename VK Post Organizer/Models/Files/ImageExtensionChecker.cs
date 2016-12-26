using System.Collections.Generic;
using JetBrains.Annotations;

namespace vk.Models.Files {
   [UsedImplicitly]
   public class ImageExtensionChecker : ExtensionChecker {
      public override IEnumerable<string> GetValidExtensions() {
         return new[] { ".jpg", ".jpeg", ".bmp", ".gif", ".png" };
      }

      public override string GetFileFilter() {
         return "Images (*.jpg, *.jpeg, *.bmp, *.gif, *.png)|*.jpg;*.jpeg;*.bmp;*.gif;*.png|All files (*.*)|*.*";
      }
   }
}