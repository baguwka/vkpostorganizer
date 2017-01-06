using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vk.Models.Files {
   public abstract class ExtensionChecker {
      public abstract IEnumerable<string> GetValidExtensions();
      public abstract string GetFileFilter();

      public bool IsFileHaveValidExtension(string file) {
         var ext = Path.GetExtension(file);
         return GetValidExtensions().Contains(ext);
      }

      public bool IsValid(string ext) {
         return GetValidExtensions().Contains(ext);
      }
   }
}