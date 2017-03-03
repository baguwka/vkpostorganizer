using System.Net.Http;
using JetBrains.Annotations;

namespace vk.Models {
   [UsedImplicitly]
   public class HttpClientHandlerFactory {
      private readonly ProxyProvider _proxyProvider;

      public HttpClientHandlerFactory(ProxyProvider proxyProvider) {
         _proxyProvider = proxyProvider;
      }

      public HttpClientHandler BuildHttpClientHandlerWithProxyIfEnabled() {
         if (_proxyProvider.ProxySettings.UseProxy) {
            var proxy = _proxyProvider.GetProxy();

            return new HttpClientHandler {
               Proxy = proxy,
               UseProxy = true,
               PreAuthenticate = true,
               UseDefaultCredentials = false
            };
         }
         else {
            return new HttpClientHandler {
               Proxy = null,
               UseProxy = false
            };
         }
      }
   }
}