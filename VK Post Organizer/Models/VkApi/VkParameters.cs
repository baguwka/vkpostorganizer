using System.Collections.Specialized;

namespace vk.Models.VkApi {
   public class VkParameters {
      public NameValueCollection Query { get; }

      public static VkParameters New() {
         return new VkParameters();
      }

      public VkParameters() {
         Query = new NameValueCollection();
      }

      public VkParameters AddParameter(string paramName, string paramValue) {
         if (!string.IsNullOrEmpty(paramValue)) {
            Query[paramName] = paramValue;
         }
         return this;
      }

      public VkParameters AddParameter(string paramName, object paramValue) {
         return AddParameter(paramName, paramValue.ToString());
      }
   }
}