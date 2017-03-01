using System;
using System.Security.Policy;
using Prism.Mvvm;

namespace vk.Models {
   [Serializable]
   public class ProxySettings : BindableBase {
      private bool _useProxy;
      private string _proxyAddress;
      private int _proxyPort;
      private Uri _proxyUri;
      private string _username;
      private string _password;

      public void Set(ProxySettings proxySettings) {
         UseProxy = proxySettings.UseProxy;
         ProxyAddress = proxySettings.ProxyAddress;
         ProxyPort = proxySettings.ProxyPort;
         Username = proxySettings.Username;
         Password = proxySettings.Password;
      }

      public bool UseProxy {
         get { return _useProxy; }
         set { SetProperty(ref _useProxy, value); }
      }

      public string ProxyAddress {
         get { return _proxyAddress; }
         set {
            var uri = new UriBuilder(!string.IsNullOrEmpty(value) ? value : "127.0.0.1").Uri;
            if (SetProperty(ref _proxyAddress, uri.ToString())) {
               updateUri();
            }
         }
      }

      public int ProxyPort {
         get { return _proxyPort; }
         set {
            if (value < 0) value = 0;
            if (value > 65535) value = 65535;
            if (SetProperty(ref _proxyPort, value)) {
               updateUri();
            }
         }
      }

      public Uri ProxyUri {
         get {
            if (_proxyUri == null) {
               updateUri();
            }
            return _proxyUri;
         }
      }

      public string Username {
         get { return _username; }
         set {
            if (value == null) value = string.Empty;
            SetProperty(ref _username, value);
         }
      }

      public string Password {
         get { return _password; }
         set {
            if (value == null) value = string.Empty;
            SetProperty(ref _password, value);
         }
      }

      private void updateUri() {
         if (UseProxy == false) {
            _proxyUri = new UriBuilder("127.0.0.1").Uri;
            return;
         }

         var builder = new UriBuilder(ProxyAddress) {Port = ProxyPort};
         _proxyUri = builder.Uri;
      }
   }
}