using System;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace vk.Models {
   public class WallItem : BindableBase {
      private IWallHolder _wallHolder;

      public ICommand ClickCommand { get; set; }

      public Action<WallItem> ClickHandler { get; set; }

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set { SetProperty(ref _wallHolder, value); }
      }

      public WallItem([NotNull] IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         WallHolder = wallHolder;
         ClickCommand = new DelegateCommand(clickCommandExecute);
      }

      private void clickCommandExecute() {
         ClickHandler?.Invoke(this);
      }
   }
}