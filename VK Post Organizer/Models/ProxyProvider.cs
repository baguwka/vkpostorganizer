using System;
using System.Net;
using JetBrains.Annotations;
using static vk.Utils.UrlHelper;

namespace vk.Models {
   [UsedImplicitly]
   public class ProxyProvider {
      public ProxySettings ProxySettings { get; private set; }

      public ProxyProvider([NotNull] Settings settings) {
         if (settings == null) {
            throw new ArgumentNullException(nameof(settings));
         }
         if (settings.Proxy == null) {
            throw new ArgumentNullException(nameof(settings.Proxy));
         }

         ProxySettings = settings.Proxy;
      }

      [CanBeNull]
      public IWebProxy GetProxy() {
         if (ProxySettings.UseProxy) {
            if (IsUrlIsValid(ProxySettings.ProxyAddress) && ProxySettings.ProxyPort > 0) {
               return Utils.UrlHelper.GetProxy(ProxySettings.ProxyUri, ProxySettings.Username, ProxySettings.Password);
            }
         }

         return null;
      }
   }
}
