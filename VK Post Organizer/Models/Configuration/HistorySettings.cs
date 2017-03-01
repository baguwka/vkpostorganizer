using System;
using Prism.Mvvm;

namespace vk.Models {
   [Serializable]
   public class HistorySettings : BindableBase {
      private bool _use;
      private Uri _uri;
      private string _url;
      private int _port;

      public void Set(HistorySettings otherHistory) {
         Use = otherHistory.Use;
         Url = otherHistory.Url;
         Port = otherHistory.Port;
      }

      public bool Use {
         get { return _use; }
         set { SetProperty(ref _use, value); }
      }

      public string Url {
         get { return _url; }
         set
         {
            var uri = new UriBuilder(!string.IsNullOrEmpty(value) ? value : "127.0.0.1").Uri;
            SetProperty(ref _url, uri.ToString());
            updateUri();
         }
      }

      public Uri Uri {
         get {
            if (_uri == null) {
               updateUri();
            }
            return _uri;
         }
      }

      public int Port {
         get { return _port; }
         set {
            if (value < 0) value = 0;
            if (value > 65535) value = 65535;
            SetProperty(ref _port, value);
            updateUri();
         }
      }

      private void updateUri() {
         if (Use == false) {
            _uri = new UriBuilder("127.0.0.1").Uri;
            return;
         }
         var builder = new UriBuilder(Url) {Port = Port};
         _uri = builder.Uri;
      }
   }
}