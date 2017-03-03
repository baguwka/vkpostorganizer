using JetBrains.Annotations;
using Prism.Mvvm;

namespace vk.Models {
   [UsedImplicitly]
   public class SharedWallContext : BindableBase {
      private IWallHolder _selectedWallHolder;

      public SharedWallContext() {
         _selectedWallHolder = new WallHolder(0);
      }

      public IWallHolder SelectedWallHolder {
         get { return _selectedWallHolder; }
         set { SetProperty(ref _selectedWallHolder, value); }
      }
   }
}