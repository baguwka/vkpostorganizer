using System;
using System.Net;
using System.Windows;
using JetBrains.Annotations;

namespace vk.Utils {
   public static class UrlHelper {
      public static bool IsUrlIsValid(string url) {
         if (!string.IsNullOrEmpty(url)) {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && uriResult.Scheme == Uri.UriSchemeHttp;

         }
         return false;
      }

      [CanBeNull]
      public static IWebProxy GetProxy(Uri uri, string username, string password) {
         if (IsUrlIsValid(uri.ToString()) && uri.Port > 0) {
            var credentials = new NetworkCredential(username, password);
            //MessageBox.Show($"url: {uri}\nusrnm: {credentials.UserName}\n pswd: {credentials.Password}");
            return new WebProxy(uri, true, null, credentials);
         }
         return null;
      }
   }
}