using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   public abstract class VkApiBase {
      public VkApiBase([NotNull] AccessToken token, [NotNull] IWebClient webClient) {
         WebClient = webClient;
         Token = token;
      }

      public IWebClient WebClient { get; }
      public AccessToken Token { get; }

      protected void checkForErrors(string response) {
         if (response.Substring(2, 5) == "error") {
            var error = deserializeError(response);
            throw new VkException($"Error code: {error.ErrorCode}\n{error.ErrorMessage}");
         }
      }

      public string ExecuteMethod(string method, VkParam parameters) {
         return ExecuteMethod(method, parameters.Result());
      }

      public string ExecuteMethod(string method, string parameters = "") {
         //if (!string.IsNullOrEmpty(parameters)) {
         //   parameters = $"&{parameters}";
         //}
         return WebClient.DownloadString( "https://api.vk.com/method/" +
                                         $"{method}" +
                                         $"?access_token={Token.Token}" +
                                         $"{parameters}" +
                                          "&v=5.60");
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