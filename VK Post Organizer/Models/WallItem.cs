using System;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Mvvm;

namespace vk.Models {
   public class WallItem : BindableBase {
      private IWallHolder _wallHolder;

      public ICommand ClickCommand { get; set; }
      private string _photo;
      private string _name;

      public event EventHandler<WallItem> Clicked;

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set { SetProperty(ref _wallHolder, value); }
      }

      public string Photo {
         get { return _photo; }
         set { SetProperty(ref _photo, value); }
      }

      public string Name {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      public WallItem([NotNull] IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         WallHolder = wallHolder;
         ClickCommand = new DelegateCommand(() => OnClicked(this));

         Name = WallHolder.Name;
         Photo = WallHolder.Photo50;
      }

      protected virtual void OnClicked(WallItem e) {
         Clicked?.Invoke(this, e);
      }
   }
}