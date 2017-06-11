using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace vk.ViewModels {
   public class ImageItem : BindableBase {
      private string _preview;
      private string _source;

      public ImageItem(string preview, string source) {
         ClickCommand = new DelegateCommand(() => {
               System.Diagnostics.Process.Start(Source);
            }, () => !string.IsNullOrEmpty(Source))
            .ObservesProperty(() => Source);

         _preview = preview;
         _source = source;
      }

      public string Preview {
         get { return _preview; }
         set { SetProperty(ref _preview, value); }
      }

      public string Source {
         get { return _source; }
         set { SetProperty(ref _source, value); }
      }

      public ICommand ClickCommand { get; set; }
   }
}