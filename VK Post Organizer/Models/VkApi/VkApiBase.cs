using System;
using System.Web;
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

      public string ExecuteMethod(string method, VkParameters parameters = null) {
         var uriBuilder = new UriBuilder($"https://api.vk.com/method/{method}");

         var uriParameters = HttpUtility.ParseQueryString(string.Empty);

         if (parameters != null) {
            uriParameters = HttpUtility.ParseQueryString(parameters.Parameters.ToString());
         }

         uriParameters.Add("access_token", Token.Token);
         uriParameters.Add("v", "5.60");

         uriBuilder.Query = uriParameters.ToString();

         var finalUri = uriBuilder.Uri;

         return WebClient.DownloadString(finalUri);
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