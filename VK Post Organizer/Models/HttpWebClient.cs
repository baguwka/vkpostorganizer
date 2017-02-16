//using System;
//using System.Net.Http;
//using System.Threading.Tasks;

//namespace vk.Models {
//   public class HttpWebClient : IWebClient {
//      private HttpClient _httpClient;
//      private readonly Settings _settings;
//      private readonly ProxyProvider _proxyProvider;

//      public HttpWebClient(Settings settings, ProxyProvider proxyProvider) {
//         _settings = settings;
//         _proxyProvider = proxyProvider;
//         _settings.Proxy.ProxySettingsChanged += (sender, args) => updateHttpClient();

//         updateHttpClient();
//      }

//      private void updateHttpClient() {
//         var httpHandler = new HttpClientHandler();

//         if (_settings.Proxy.UseProxy) {
//            var myProxy = _proxyProvider.GetProxy();
//            if (myProxy != null) {
//               httpHandler.Proxy = myProxy;
//               httpHandler.UseProxy = true;
//            }
//         }
//         else {
//            httpHandler.UseProxy = false;
//         }

//         _httpClient = new HttpClient(httpHandler);
//      }

//      public string DownloadString(Uri address) {
//         throw new NotImplementedException();
//      }

//      public string DownloadString(string address) {
//         throw new NotImplementedException();
//      }

//      public async Task<string> DownloadStringAsync(Uri address) {
//         var response = await _httpClient.GetAsync(address);
//         return await response.Content.ReadAsStringAsync();
//      }

//      public async Task<string> DownloadStringAsync(string adress) {
//         return await DownloadStringAsync(new Uri(adress));
//      }
//   }
//}