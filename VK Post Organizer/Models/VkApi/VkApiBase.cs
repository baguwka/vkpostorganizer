using System.Net;
using System.Text;

namespace vk.Models.VkApi {
   public abstract class VkApiBase {
      protected string token { get; }

      public VkApiBase(string token) {
         this.token = token;
      }

      public string ExecuteMethod(string method, string parameters) {
         using (var wc = new WebClient()) {
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString($"https://api.vk.com/method/{method}?{parameters}&access_token={token}");
         }
      }
   }
}