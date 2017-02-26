using JetBrains.Annotations;
using Prism.Events;
using vk.ViewModels;

namespace vk.Events {
   [UsedImplicitly]
   public class UploaderEvents {
      public class SetVisibility : PubSubEvent<bool> { }
      public class Configure : PubSubEvent<UploaderViewModelConfiguration> { }
      public class BusyEvent : PubSubEvent<bool> { }
   }
}