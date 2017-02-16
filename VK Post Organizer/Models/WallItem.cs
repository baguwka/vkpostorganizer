using System;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Mvvm;

namespace vk.Models {
   public class WallItem : BindableBase {
      private IWallHolder _wallHolder;

      public ICommand ClickCommand { get; set; }

      public event EventHandler<WallItem> Clicked;

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set { SetProperty(ref _wallHolder, value); }
      }

      public WallItem([NotNull] IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         WallHolder = wallHolder;
         ClickCommand = new DelegateCommand(() => OnClicked(this));
      }

      protected virtual void OnClicked(WallItem e) {
         Clicked?.Invoke(this, e);
      }
   }
}