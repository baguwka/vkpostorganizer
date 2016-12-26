using System.Collections.Generic;
using System.Linq;

namespace vk.Models.Files {
   public abstract class ExtensionChecker {
      public abstract IEnumerable<string> GetValidExtensions();
      public abstract string GetFileFilter();

      public bool IsValid(string ext) {
         return GetValidExtensions().Contains(ext);
      }
   }
}