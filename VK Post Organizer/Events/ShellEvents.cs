
using Prism.Events;

namespace vk.Events {
   public class ShellEvents {
      public class BusyEvent : PubSubEvent<bool> { }
      public class WallSelectedEvent : PubSubEvent<bool> { }
   }
}