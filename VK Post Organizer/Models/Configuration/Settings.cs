using System;
using Microsoft.Practices.Prism.Mvvm;

namespace vk.Models.Configuration {
   [Serializable]
   public class UploadSettings : BindableBase {
      private bool _closeUploadWindowAfterPublish;
      private bool _postFromGroup;
      private bool _signedPosting;

      public bool SignedPosting {
         get { return _signedPosting; }
         set { SetProperty(ref _signedPosting, value); }
      }

      public bool CloseUploadWindowAfterPublish {
         get { return _closeUploadWindowAfterPublish; }
         set { SetProperty(ref _closeUploadWindowAfterPublish, value); }
      }

      public bool PostFromGroup {
         get { return _postFromGroup; }
         set { SetProperty(ref _postFromGroup, value); }
      }

      public void Set(UploadSettings other) {
         CloseUploadWindowAfterPublish = other.CloseUploadWindowAfterPublish;
      }
   }

   [Serializable]
   public class Settings : BindableBase {
      public Settings() {
         Proxy = new ProxySettings();
         Upload = new UploadSettings();
      }

      public ProxySettings Proxy { get; set; }
      public UploadSettings Upload { get; set; }

      public void ApplySettings(Settings other) {
         if (other == null) return;

         Proxy.Set(other.Proxy);
         Upload.Set(other.Upload);
      }
   }
}