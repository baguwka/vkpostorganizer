using System.Text;

namespace vk.Models.VkApi {
   public class VkParam {
      public static VkParam New() {
         return new VkParam();
      }

      private readonly StringBuilder _parameters;

      public VkParam() {
         _parameters = new StringBuilder();
      }

      public string Result() {
         return _parameters.ToString();
      }

      public VkParam AddParam(string paramName, string paramValue) {
         if (!string.IsNullOrEmpty(paramValue)) {
            _parameters.Append($"&{paramName}={paramValue}");
         }
         return this;
      }

      public VkParam AddParam(string paramName, object paramValue) {
         return AddParam(paramName, paramValue.ToString());
      }
   }
}