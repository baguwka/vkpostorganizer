using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   public abstract class VkApiBase {
      private readonly IWebClient _webClient;
      private readonly AccessToken _token;

      public VkApiBase([NotNull] AccessToken token, [NotNull] IWebClient webClient) {
         _webClient = webClient;
         _token = token;
      }

      protected void checkForErrors(string response) {
         if (response.Substring(2, 5) == "error") {
            var error = deserializeError(response);
            throw new VkException($"Error code: {error.ErrorCode}\n{error.ErrorMessage}");
         }
      }

      public string ExecuteMethod(string method, string parameters) {
         return _webClient.DownloadString($"https://api.vk.com/method/{method}?{parameters}&access_token={_token.Token}&v=5.60");
      }

      protected Error deserializeError(string jsonString) {
         return JsonConvert.DeserializeObject<ErrorResponse>(jsonString).Error;
      }
   }

   [UsedImplicitly]
   public class ErrorResponse {
      [JsonProperty(PropertyName = "error")]
      public Error Error { get; set; }
   }
}