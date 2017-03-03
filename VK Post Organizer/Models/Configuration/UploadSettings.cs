using System;
using Prism.Mvvm;

namespace vk.Models {
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
         PostFromGroup = other.PostFromGroup;
         SignedPosting = other.SignedPosting;
      }
   }
}