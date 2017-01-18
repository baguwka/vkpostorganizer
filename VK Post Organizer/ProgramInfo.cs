using System.Reflection;
using System.Runtime.InteropServices;

namespace vk {
   public static class ProgramInfo {
      public static string AssemblyGuid {
         get {
            var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof (GuidAttribute), false);
            var guid = attributes.Length == 0
               ? string.Empty
               : ((GuidAttribute)attributes[0]).Value;
            return guid;
         }
      }
   }
}