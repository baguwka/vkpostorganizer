using System.Collections.Specialized;
using System.Web;

namespace vk.Models.VkApi {
   public class VkParameters {
      public NameValueCollection Parameters { get; }

      public static VkParameters New() {
         return new VkParameters();
      }

      public VkParameters() {
         Parameters = HttpUtility.ParseQueryString(string.Empty);
      }

      public string Result() {
         return string.Join("&", Parameters);
      }

      public VkParameters AddParam(string paramName, string paramValue) {
         if (!string.IsNullOrEmpty(paramValue)) {
            Parameters.Add(paramName, paramValue);
         }
         return this;
      }

      public VkParameters AddParam(string paramName, object paramValue) {
         return AddParam(paramName, paramValue.ToString());
      }
   }
}