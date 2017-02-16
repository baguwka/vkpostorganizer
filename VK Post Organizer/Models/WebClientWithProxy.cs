using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace vk.Models {
   [UsedImplicitly]
   public class WebClientWithProxy : IWebClient {
      private readonly Settings _settings;
      private readonly ProxyProvider _proxyProvider;

      public WebClientWithProxy(Settings settings, ProxyProvider proxyProvider) {
         _settings = settings;
         _proxyProvider = proxyProvider;
      }

      private void setProxyIfPossible(WebClient wc) {
         if (_settings.Proxy.UseProxy) {
            var myProxy = _proxyProvider.GetProxy();
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
         return await DownloadStringAsync(new Uri(adress));
         //using (var wc = new WebClient()) {
         //   setProxyIfPossible(wc);
         //   wc.Encoding = Encoding.UTF8;
         //   return await wc.DownloadStringTaskAsync(adress);
         //}
      }
   }
}