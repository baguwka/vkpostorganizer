using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace vk.Models {
   [UsedImplicitly]
   public class WebClientWithProxy : IWebClient {
      private readonly Settings _settings;
      private readonly ProxyProvider _proxyProvider;
      private readonly HttpClient _httpClient;

      public WebClientWithProxy(Settings settings, ProxyProvider proxyProvider) {
         _settings = settings;
         _proxyProvider = proxyProvider;
         
         _httpClient = new HttpClient();
      }

      private void setProxyIfPossible(WebClient wc) {
         if (_settings.Proxy.UseProxy) {
            var myProxy = _proxyProvider.GetProxy();
            if (myProxy != null) {
               wc.Proxy = myProxy;
            }
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
      }
   }
}