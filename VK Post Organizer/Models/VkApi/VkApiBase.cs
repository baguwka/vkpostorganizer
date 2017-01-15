using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using Microsoft.Practices.Unity;
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
         if (string.IsNullOrEmpty(response)) {
            throw new VkException($"Got no response from server");
         }

         if (response.Substring(2, 5) == "error") {
            var error = deserializeError(response);
            throw new VkException($"Error code: {error.ErrorCode}\n{error.ErrorMessage}");
         }
      }

      public async Task<string> ExecuteMethodAsync(string method, VkParameters parameters = null) {
         var finalUri = buildFinalUri(method, parameters);
         try {
            var result = await WebClient.DownloadStringAsync(finalUri);
            return result;
         }
         catch (WebException ex) {
            if (tryToHandleException(ex) == false) {
               throw;
            }
         }
         return string.Empty;
      }

      public string ExecuteMethod(string method, VkParameters parameters = null) {
         var finalUri = buildFinalUri(method, parameters);
         try {
            var result = WebClient.DownloadString(finalUri);
            return result;
         }
         catch (WebException ex) {
            if (tryToHandleException(ex) == false) {
               throw;
            }
         }
         return string.Empty;
      }

      private bool tryToHandleException(WebException ex) {
         string message;
         var innerException = ex.InnerException;

         switch (ex.Status) {
            case WebExceptionStatus.ConnectFailure:
               var proxyUsed = App.Container.Resolve<Settings>().Proxy.UseProxy;
               message = $"{innerException?.Message ?? "Ошибка соединения с удаленным сервером."}{(proxyUsed ? "\n\nCheck your proxy server" : "")}";
               break;
            default:
               return false;
         }

         throw new VkException(!string.IsNullOrEmpty(message) ? message : innerException?.Message ?? ex.Message, ex);
      }

      private Uri buildFinalUri(string method, VkParameters parameters) {
         var uriBuilder = new UriBuilder($"https://api.vk.com/method/{method}");

         var uriParameters = new NameValueCollection();

         if (parameters != null) {
            uriParameters.Add(parameters.Query);
         }

         uriParameters["access_token"] = Token.Token;
         uriParameters["v"] = "5.60";

         uriBuilder.Query = string.Join("&", uriParameters.AllKeys
            .Select(key => $"{key}={HttpUtility.UrlEncode(uriParameters[key])}"));

         return uriBuilder.Uri;
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