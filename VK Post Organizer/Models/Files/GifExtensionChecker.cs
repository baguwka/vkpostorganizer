using System.Collections.Generic;
using JetBrains.Annotations;

namespace vk.Models.Files {
   [UsedImplicitly]
   public class GifExtensionChecker : ExtensionChecker {
      public override IEnumerable<string> GetValidExtensions() {
         return new[] { ".gif" };
      }

      public override string GetFileFilter() {
         return "Images (*.gif)|*.gif;|All files (*.*)|*.*";
      }
   }
}