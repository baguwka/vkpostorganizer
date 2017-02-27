using JetBrains.Annotations;
using Prism.Events;

namespace vk.Events {
   [UsedImplicitly]
   public class ContentEvents {
      public class BusyEvent : PubSubEvent<bool> { }
      public class LeftBlockExpandAllRequest : PubSubEvent { }
      public class LeftBlockCollapseAllRequest : PubSubEvent { }
      public class ContentRegionBusyEvent : PubSubEvent<bool> { }
   }
}