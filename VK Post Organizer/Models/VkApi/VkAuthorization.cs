using System;
using System.Web;
using System.Windows;
using JetBrains.Annotations;
using Prism.Mvvm;
using vk.Models.VkApi;

namespace vk.Models {
   public class AuthorizationResponse {
      public bool Successful { get; set; }
      public string TokenResponse { get; set; }
      public int UserId { get; set; }
   }

   [UsedImplicitly]
   public class VkAuthorization : BindableBase {
      public const string SCOPES = "offline,wall,groups,photos,docs";
      public const string CLIENT_ID = "5590028";

      public static void ClearCookies() {
         DeleteSingleCookie("remixsid");
         DeleteSingleCookie("remixlhk");
      }

      public static void DeleteSingleCookie(string name) {
         try {
            var expiration = DateTime.UtcNow.AddDays(-1);
            string cookie = $"{name}=; expires={expiration:R}; path=/; domain=.vk.com";
            Application.SetCookie(new Uri("https://www.vk.com"), cookie);
         }
         catch {
            // ignored
         }
      }

      public static void ClearAllCookies() {
         try {
            var cookies = Application.GetCookie(new Uri("https://www.vk.com"));
            var values = cookies?.Split(';');

            if (values == null) return;

            foreach (var value in values) {
               DeleteSingleCookie(value);
            }
         }
         catch {
            // ignored
         }
         finally {
            //App.SuppressWininetBehavior();
         }
      }

      public static Uri GetLogoutUri() {
          //_webClient.DownloadStringAsync("http://api.vk.com/oauth/logout");
         var uriBuilder = new UriBuilder("https://oauth.vk.com/oauth/logout");
         var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
         //queryBuilder["response_type"] = "token";
         queryBuilder["redirect_uri"] = "oauth.vk.com/blank.html";

         queryBuilder["client_id"] = CLIENT_ID;
         queryBuilder["scope"] = SCOPES;
         queryBuilder["display"] = "popup";
         queryBuilder["v"] = VkApi.VkApi.VERSION;

         uriBuilder.Query = queryBuilder.ToString();
         return uriBuilder.Uri;
      }

      /// <summary>
      /// The destination url you should call to get the token
      /// </summary>
      /// <returns></returns>
      public static Uri GetAuthorizationUri() {
         var uriBuilder = new UriBuilder("https://oauth.vk.com/authorize");
         var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
         queryBuilder["response_type"] = "token";
         queryBuilder["redirect_uri"] = "oauth.vk.com/blank.html";

         queryBuilder["client_id"] = CLIENT_ID;
         queryBuilder["scope"] = SCOPES;
         queryBuilder["display"] = "popup";
         queryBuilder["v"] = VkApi.VkApi.VERSION;

         uriBuilder.Query = queryBuilder.ToString();
         return uriBuilder.Uri;
      }

      public AuthorizationResponse ParseResultUrl(string url) {
         var success = false;
         var token = string.Empty;
         var userId = 0;

         // replacing # to ? so we can parse the query
         url = url.Replace('#', '?');
         var uri = new Uri(url);
         var queryBuilder = HttpUtility.ParseQueryString(uri.Query);

         try {
            token = queryBuilder["access_token"];
            userId = int.Parse(queryBuilder["user_id"]);
            success = !string.IsNullOrEmpty(token) && userId > 0;
         }
         catch {
            success = false;
         }

         return new AuthorizationResponse { Successful = success, TokenResponse = token, UserId = userId };
      }

      public AuthorizationResponse TryToSetupTokenFromUrl(AccessToken token, string url) {
         var response = ParseResultUrl(url);
         if (response.Successful) {
            SetupYourToken(token, response.TokenResponse, response.UserId);
         }
         return response;
      }

      public void SetupYourToken(AccessToken token, string tokenString, int userId) {
         token.Token = tokenString;
         token.UserID = userId;
      }
   }
}