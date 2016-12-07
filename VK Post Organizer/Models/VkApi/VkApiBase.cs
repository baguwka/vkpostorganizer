using JetBrains.Annotations;

namespace vk.Models.VkApi {
   public abstract class VkApiBase {
      private readonly IWebClient _webClient;
      private readonly AccessToken _token;

      public VkApiBase([NotNull] AccessToken token, [NotNull] IWebClient webClient) {
         _webClient = webClient;
         _token = token;
      }

      public string ExecuteMethod(string method, string parameters) {
         return _webClient.DownloadString($"https://api.vk.com/method/{method}?{parameters}&access_token={_token.Token}&v=5.60");
      }
   }
}