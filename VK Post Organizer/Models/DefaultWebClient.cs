using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace vk.Models {
   [UsedImplicitly]
   public class DefaultWebClient : IWebClient {
      public string DownloadString(Uri adress) {
         using (var wc = new WebClient()) {
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(adress);
         }
      }

      public string DownloadString(string adress) {
         using (var wc = new WebClient()) {
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(adress);
         }
      }

      public Task<string> DownloadStringAsync(Uri adress) {
         using (var wc = new WebClient()) {
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadStringTaskAsync(adress);
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