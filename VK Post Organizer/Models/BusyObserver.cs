using JetBrains.Annotations;
using Prism.Events;
using Prism.Mvvm;
using vk.Events;

namespace vk.Models {
   [UsedImplicitly]
   public class BusyObserver : BindableBase {
      private bool _shellIsBusy;
      private bool _contentIsBusy;
      private bool _uploaderIsBusy;

      public bool ShellIsBusy {
         get { return _shellIsBusy; }
         set { SetProperty(ref _shellIsBusy, value); }
      }

      public bool ContentIsBusy {
         get { return _contentIsBusy; }
         set { SetProperty(ref _contentIsBusy, value); }
      }

      public bool UploaderIsBusy {
         get { return _uploaderIsBusy; }
         set { SetProperty(ref _uploaderIsBusy, value); }
      }

      public BusyObserver(IEventAggregator eventAggregator) {
         eventAggregator.GetEvent<ShellEvents.BusyEvent>().Subscribe(busy => {
            ShellIsBusy = busy;
         });

         eventAggregator.GetEvent<ContentEvents.BusyEvent>().Subscribe(busy => {
            ContentIsBusy = busy;
         });

         eventAggregator.GetEvent<UploaderEvents.BusyEvent>().Subscribe(busy => {
            UploaderIsBusy = busy;
         });
      }
   }
}