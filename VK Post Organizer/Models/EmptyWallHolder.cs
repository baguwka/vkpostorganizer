using JetBrains.Annotations;

namespace vk.Models {
   [UsedImplicitly]
   public class EmptyWallHolder : IWallHolder {
      public int ID { get; set; }
      public string Name { get; set; }
      public string Photo50 { get; set; }
      public string Photo200 { get; set; }
   }
}