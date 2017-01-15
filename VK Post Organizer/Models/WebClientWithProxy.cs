using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Practices.Unity;

namespace vk.Models {
   public class LowTimeoutWebClient : WebClient {
      protected override WebRequest GetWebRequest(Uri address) {
         var request = base.GetWebRequest(address);

         if (request != null) {
            request.Timeout = 3 * 1000;
         }

         return request;
      }
   }

   [UsedImplicitly]
   public class WebClientWithProxy : IWebClient {
      private void setProxyIfPossible(WebClient wc) {
         var settings = App.Container.Resolve<Settings>();
         if (settings.Proxy.UseProxy) {
            var myProxy = App.Container.Resolve<ProxyProvider>().GetProxy();
            if (myProxy != null) {
               wc.Proxy = myProxy;
            }
         }
      }

      public string DownloadString(Uri address) {
         using (var wc = new WebClient()) {
            setProxyIfPossible(wc);
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(address);
         }
      }

      public string DownloadString(string address) {
         using (var wc = new WebClient()) {
            setProxyIfPossible(wc);
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(address);
         }
      }

      public async Task<string> DownloadStringAsync(Uri address) {
         using (var wc = new WebClient()) {
            setProxyIfPossible(wc);
            wc.Encoding = Encoding.UTF8;
            return await wc.DownloadStringTaskAsync(address);
         }
      }

      public async Task<string> DownloadStringAsync(string adress) {
         using (var wc = new WebClient()) {
            setProxyIfPossible(wc);
            wc.Encoding = Encoding.UTF8;
            return await wc.DownloadStringTaskAsync(adress);
         }
      }
   }
}