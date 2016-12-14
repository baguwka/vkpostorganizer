using JetBrains.Annotations;

namespace vk.Models {
   [UsedImplicitly]
   public class EmptyWallHolder : IWallHolder {
      public int ID { get; }
      public string Name { get; }
      public string Photo50 { get; }
      public string Photo200 { get; }
   }
}