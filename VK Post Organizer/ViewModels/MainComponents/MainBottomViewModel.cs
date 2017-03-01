using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using vk.Events;
using vk.Models;

// ReSharper disable MemberCanBePrivate.Global

namespace vk.ViewModels {
   public class MainBottomViewModel : BindableBase {
      private readonly BusyObserver _busyObserver;
      public ICommand BackCommand { get; }
      public ICommand RefreshCommand { get; }
      public ICommand SettingsCommand { get; }

      private bool _isBusy;
      private bool _contentIsBusy;
      private bool _uploaderisBusy;
      private bool _isWallSelected;
      private string _version;

      public bool IsBusy {
         get { return _isBusy; }
         set { SetProperty(ref _isBusy, value); }
      }
      
      public bool IsWallSelected {
         get { return _isWallSelected; }
         set { SetProperty(ref _isWallSelected, value); }
      }

      public bool ContentIsBusy {
         get { return _contentIsBusy; }
         private set { SetProperty(ref _contentIsBusy, value); }
      }

      public bool UploaderIsBusy {
         get { return _uploaderisBusy; }
         private set { SetProperty(ref _uploaderisBusy, value); }
      }

      public string Version {
         get { return _version; }
         set { SetProperty(ref _version, value); }
      }

      public MainBottomViewModel(IEventAggregator eventAggregator, BusyObserver busyObserver) {
         var aggregator = eventAggregator;
         _busyObserver = busyObserver;
         _busyObserver.PropertyChanged += (sender, args) => {
            ContentIsBusy = _busyObserver.ContentIsBusy;
            UploaderIsBusy = _busyObserver.UploaderIsBusy;
         };

         aggregator.GetEvent<ShellEvents.WallSelectedEvent>().Subscribe(onShellWallSelectedChanged);

         BackCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.Back>().Publish();
         }, () => !IsBusy && !ContentIsBusy && !UploaderIsBusy && IsWallSelected)
         .ObservesProperty(() => IsBusy)
         .ObservesProperty(() => ContentIsBusy)
         .ObservesProperty(() => UploaderIsBusy)
         .ObservesProperty(() => IsWallSelected);

         RefreshCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.Refresh>().Publish();
         }, () => !IsBusy && !ContentIsBusy && !UploaderIsBusy)
         .ObservesProperty(() => IsBusy)
         .ObservesProperty(() => ContentIsBusy)
         .ObservesProperty(() => UploaderIsBusy);

         SettingsCommand = new DelegateCommand(() => {
            aggregator.GetEvent<MainBottomEvents.Settings>().Publish();
         });

         Version = App.Version.ToString();
      }

      private void onShellWallSelectedChanged(bool wallSelected) {
         IsWallSelected = wallSelected;
      }
   }
}