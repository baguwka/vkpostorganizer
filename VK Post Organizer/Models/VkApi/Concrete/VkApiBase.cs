using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   public abstract class VkApiBase {
      public const string VERSION = "5.62";

      public VkApiBase([NotNull] AccessToken token, [NotNull] IWebClient webClient) {
         WebClient = webClient;
         Token = token;
      }

      public IWebClient WebClient { get; }
      public AccessToken Token { get; }

      private void checkForErrors(string response) {
         if (string.IsNullOrEmpty(response) || response.Length < 5) {
            throw new VkException($"Got no response from server");
         }

         if (response.Substring(2, 5) == "error") {
            var error = deserializeError(response);
            throw new VkException($"Error code: {error.ErrorCode}\n{error.ErrorMessage}", error.ErrorCode);
         }
      }

      private int _failedAttempts = 0;

      public async Task<string> ExecuteMethodAsync(string method, [NotNull] VkParameters query) {
         if (query == null) {
            throw new ArgumentNullException(nameof(query));
         }

         var finalUri = buildFinalUri(method, query);
         var result = string.Empty;
         try {
            result = await WebClient.DownloadStringAsync(finalUri);
            checkForErrors(result);
            _failedAttempts = 0;
            return result;
         }
         catch (WebException ex) {
            if (tryToHandleException(ex) == false) {
               throw;
            }
         }
         catch (VkException ex) {
            if (_failedAttempts > 5) {
               throw;
            }

            _failedAttempts++;
            //too much requests per second
            if (ex.ErrorCode == 6) {
               await Task.Delay(TimeSpan.FromSeconds(0.3f));
               return await ExecuteMethodAsync(method, query);
            }

            throw;
         }
         //catch (VkException ex) {
         //   //captcha needed
         //   if (ex.ErrorCode == 14) {
         //      // do captcha
         //      doCaptcha(parameters, result);
         //      await ExecuteMethodAsync(method, parameters);
         //   }
         //}
         return string.Empty;
      }

      //private void doCaptcha(VkParameters parameters, string result) {
      //   var error = deserializeError(result);

      //   parameters.AddParameter("captcha_sid", error.CaptchaSid);
      //   parameters.AddParameter("captcha_key", 123);
      //}

      private bool tryToHandleException(WebException ex) {
         string message;
         var innerException = ex.InnerException; 

         switch (ex.Status) {
            case WebExceptionStatus.ConnectFailure:
               message = $"{innerException?.Message ?? "Ошибка соединения с удаленным сервером."}\n\nCheck your proxy server if you use one.";
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
         uriParameters["v"] = VERSION;

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