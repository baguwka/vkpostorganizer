using System;
using System.Net;
using System.Text;

namespace vk.Models {
   public class DefaultWebClient : IWebClient {
      public string DownloadString(Uri address) {
         using (var wc = new WebClient()) {
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(address);
         }
      }

      public string DownloadString(string address) {
         using (var wc = new WebClient()) {
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(address);
         }
      }
   }
}