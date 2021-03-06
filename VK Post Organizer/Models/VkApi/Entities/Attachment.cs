using JetBrains.Annotations;
using Newtonsoft.Json;
using Prism.Mvvm;
using vk.Models.UrlHelper;
using vk.ViewModels;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Attachment : BindableBase {
      private string _type;
      private Photo _photo;
      private Document _document;

      [JsonProperty(PropertyName = "type")]
      public string Type {
         get { return _type; }
         set { SetProperty(ref _type, value); }
      }

      [JsonProperty(PropertyName = "photo")]
      public Photo Photo {
         get { return _photo; }
         set { SetProperty(ref _photo, value); }
      }

      [JsonProperty(PropertyName = "doc")]
      public Document Document {
         get { return _document; }
         set { SetProperty(ref _document, value); }
      }

      public override string ToString() {
         switch (Type) {
            case "photo":
               return $"{Type}{Photo}";
            case "doc":
               return $"{Type}{Document}";
         }

         return base.ToString();
      }
   }

   public static class AttachmentExtensions {
      public static ImageItem ObtainPhotoUrl(this Attachment attachment, ImageSize imageSize, IImageUrlObtainer obtainer) {
         if (attachment.Type != "photo") {
            return null;
         }

         //var obtainer = App.Container.GetInstance<PhotoUrlObtainer>();
         return obtainer.Obtain(attachment, imageSize);
      }

      public static ImageItem ObtainDocumentPreview(this Attachment attachment, ImageSize imageSize, IImageUrlObtainer obtainer) {
         if (attachment.Type != "doc") {
            return null;
         }

         //var obtainer = App.Container.GetInstance<DocumentPreviewUrlObtainer>();
         return obtainer.Obtain(attachment, imageSize);
      }
   }
}