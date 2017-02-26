using JetBrains.Annotations;
using Prism.Events;

namespace vk.Events {
   [UsedImplicitly]
   public class ContentEvents {
      public class BusyEvent : PubSubEvent<bool> { }
   }
}