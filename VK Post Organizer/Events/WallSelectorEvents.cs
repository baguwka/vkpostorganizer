using Prism.Events;
using vk.Models;

namespace vk.Events {
   public class WallSelectedEvent : PubSubEvent<WallItem> {}
   public class FillWallListEvent : PubSubEvent {}
}