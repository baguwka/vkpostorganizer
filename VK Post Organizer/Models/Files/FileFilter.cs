using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vk.Models.Files {
   public class FileFilter {
      private readonly ExtensionChecker _checker;

      public FileFilter(ExtensionChecker checker) {
         _checker = checker;
      }

      public IEnumerable<string> FilterOut(IEnumerable<string> files) {
         return (from file in files
            let ext = Path.GetExtension(file)
            where !string.IsNullOrEmpty(ext) && _checker.IsValid(ext)
            select file)
            .ToList();
      }
   }
}