using Prism.Events;
using vk.Models;

namespace vk.Events {
   public class WallSelectorEvents {
      public class WallSelected : PubSubEvent<WallItem> { }
      public class FillWallRequest : PubSubEvent { }
   }
}