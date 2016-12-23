using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace vk.ViewModels {
   public class ImageItem : BindableBase {
      private string _url;
      private string _source;

      public ImageItem(string url, string source) {
         ClickCommand = new DelegateCommand(() => {
            if (!string.IsNullOrEmpty(Source)) {
               System.Diagnostics.Process.Start(Source);
            }
         });
         _url = url;
         _source = source;
      }

      public string Url {
         get { return _url; }
         set { SetProperty(ref _url, value); }
      }

      public string Source {
         get { return _source; }
         set { SetProperty(ref _source, value); }
      }

      public ICommand ClickCommand { get; set; }
   }
}