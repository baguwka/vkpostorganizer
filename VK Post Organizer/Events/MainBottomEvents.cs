using JetBrains.Annotations;
using Prism.Events;

namespace vk.Events {
   [UsedImplicitly]
   public class MainBottomEvents {
      public class Back : PubSubEvent { }
      public class Refresh : PubSubEvent { }
      public class Upload : PubSubEvent { }
      public class Settings : PubSubEvent {}
      public class LoggingDirectory : PubSubEvent { }
   }
}