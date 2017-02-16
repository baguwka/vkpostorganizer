using JetBrains.Annotations;
using Prism.Events;

namespace vk.Events {
   [UsedImplicitly]
   public class MainBottomEvents {
      public class BackClick : PubSubEvent { }
      public class RefreshClick : PubSubEvent { }
      public class UploadClick : PubSubEvent { }
      public class SettingsClick : PubSubEvent {}
   }
}