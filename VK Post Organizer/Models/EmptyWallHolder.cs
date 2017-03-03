using JetBrains.Annotations;

namespace vk.Models {
   [UsedImplicitly]
   public class WallHolder : IWallHolder {
      public WallHolder(int id) {
         ID = id;
      }

      public int ID { get; set; }
      public string Photo50 { get; }
      public string Name { get; }
      public string Photo200 { get; }
      public string Description { get; }
   }

   [UsedImplicitly]
   public class EmptyWallHolder : IWallHolder {
      public int ID { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public string Photo50 { get; set; }
      public string Photo200 { get; set; }
   }
}