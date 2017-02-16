using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using vk.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace vk.ViewModels {
   public class MainBottomViewModel : BindableBase {
      public ICommand BackCommand { get; }
      public ICommand RefreshCommand { get; }
      public ICommand UploadCommand { get; }
      public ICommand SettingsCommand { get; }

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
            aggregator.GetEvent<MainBottomEvents.Back>().Publish();
         }, () => !IsBusy && IsWallSelected)
         .ObservesProperty(() => IsBusy)
         .ObservesProperty(() => IsWallSelected);

         RefreshCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.Refresh>().Publish();
         }, () => !IsBusy)
         .ObservesProperty(() => IsBusy);

         UploadCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.Upload>().Publish();
         }, () => IsWallSelected)
         .ObservesProperty(() => IsWallSelected);

         SettingsCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.Settings>().Publish();
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