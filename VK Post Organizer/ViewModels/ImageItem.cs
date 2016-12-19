using Microsoft.Practices.Prism.Mvvm;

namespace vk.ViewModels {
   public class ImageItem : BindableBase {
      private string _url;

      public ImageItem(string url) {
         _url = url;
      }

      public string Url {
         get { return _url; }
         set { SetProperty(ref _url, value); }
      }
   }
}