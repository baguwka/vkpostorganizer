using System;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace vk.Models {
   [Serializable]
   public class Settings : BindableBase {
      private ProxySettings _proxy;
      private HistorySettings _history;

      public Settings() {
         Proxy = new ProxySettings {
            UseProxy = false,
         };

         History = new HistorySettings {
            Use = false,
         };

         Upload = new UploadSettings {
            CloseUploadWindowAfterPublish = true,
            SignedPosting = false,
            PostFromGroup = true
         };
      }

      public ProxySettings Proxy {
         get { return _proxy; }
         private set { SetProperty(ref _proxy, value); }
      }

      public HistorySettings History {
         get { return _history; }
         private set { SetProperty(ref _history, value); }
      }

      [JsonProperty(Required = Required.DisallowNull)]
      public UploadSettings Upload { get; set; }

      public void ApplySettings(Settings other) {
         if (other == null) return;

         Proxy.Set(other.Proxy);
         Upload.Set(other.Upload);
         History.Set(other.History);
      }
   }
}