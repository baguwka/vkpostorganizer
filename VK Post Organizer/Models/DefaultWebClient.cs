using System;
using System.Net;
using System.Text;
using JetBrains.Annotations;

namespace vk.Models {
   [UsedImplicitly]
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

      public byte[] UploadFile(string url, string method, string path) {
         using (var wc = new WebClient()) {
            wc.Encoding = Encoding.UTF8;
            return wc.UploadFile(url, method, path);
         }
      }
   }
}