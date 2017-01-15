using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

      public Task<string> DownloadStringAsync(Uri address) {
         using (var wc = new WebClient()) {
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadStringTaskAsync(address);
         }
      }

      public Task<string> DownloadStringAsync(string adress) {
         using (var wc = new WebClient()) {
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadStringTaskAsync(adress);
         }
      }
   }
}