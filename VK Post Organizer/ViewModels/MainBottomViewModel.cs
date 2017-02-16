using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using vk.Events;

namespace vk.ViewModels {
   public class MainBottomViewModel : BindableBase {
      public DelegateCommand BackCommand { get; private set; }
      public DelegateCommand RefreshCommand { get; private set; }
      public DelegateCommand UploadCommand { get; private set; }
      public DelegateCommand SettingsCommand { get; private set; }

      private bool _isBusy;
      private bool _isWallSelected;

      public bool IsBusy {
         get { return _isBusy; }
         set { SetProperty(ref _isBusy, value); }
      }
      
      public bool IsWallSelected {
         get { return _isWallSelected; }
         set { SetProperty(ref _isWallSelected, value); }
      }

      public MainBottomViewModel(IEventAggregator eventAggregator) {
         var aggregator = eventAggregator;

         aggregator.GetEvent<ShellEvents.BusyEvent>().Subscribe(onShellIsBusyChanged);
         aggregator.GetEvent<ShellEvents.WallSelectedEvent>().Subscribe(onShellWallSelectedChanged);

         BackCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.BackClick>().Publish();
         }, () => !IsBusy && IsWallSelected)
         .ObservesProperty(() => IsBusy)
         .ObservesProperty(() => IsWallSelected);

         RefreshCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.RefreshClick>().Publish();
         }, () => !IsBusy)
         .ObservesProperty(() => IsBusy);

         UploadCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.UploadClick>().Publish();
         }, () => IsWallSelected)
         .ObservesProperty(() => IsWallSelected);

         SettingsCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.SettingsClick>().Publish();
         });
      }

      private void onShellWallSelectedChanged(bool wallSelected) {
         IsWallSelected = wallSelected;
      }

      private void onShellIsBusyChanged(bool busy) {
         IsBusy = busy;
      }
   }
}