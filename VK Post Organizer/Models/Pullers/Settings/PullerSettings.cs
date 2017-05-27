namespace vk.Models.Pullers {
   public abstract class PullerSettings {
      public static PullerSettings No { get; }

      static PullerSettings() {
         No = new NoPullerSettings();
      }
   }
}