using JetBrains.Annotations;

namespace vk.Models.VkApi {
   public abstract class VkApiBase {
      private readonly IWebClient _webClient;
      protected AccessToken token { get; }

      public VkApiBase([NotNull] AccessToken token, [NotNull] IWebClient webClient) {
         _webClient = webClient;
         this.token = token;
      }

      public string ExecuteMethod(string method, string parameters) {
         return _webClient.DownloadString($"https://api.vk.com/method/{method}?{parameters}&access_token={token.Token}&v=5.60");
      }
   }
}