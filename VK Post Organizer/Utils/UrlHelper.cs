﻿using System;
using System.Net;
using JetBrains.Annotations;

namespace vk.Utils {
   public static class UrlHelper {
      public static bool IsUriIsValid(Uri uri) {
         if (uri != null) {
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;

         }
         return false;
      }

      public static bool IsUrlIsValid(string url) {
         if (!string.IsNullOrEmpty(url)) {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

         }
         return false;
      }

      [CanBeNull]
      public static IWebProxy GetProxy(Uri uri, string username, string password) {
         if (IsUrlIsValid(uri.ToString()) && uri.Port > 0) {
            var credentials = new NetworkCredential(username, password);
            return new WebProxy(uri, true, null, credentials);
         }
         return null;
      }
   }
}