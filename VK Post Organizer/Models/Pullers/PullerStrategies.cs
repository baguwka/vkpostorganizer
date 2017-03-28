using JetBrains.Annotations;

namespace vk.Models.Pullers {
   [UsedImplicitly]
   public class PullerStrategies {
      private readonly VkActualPullerStrategy _actualPullerStrategy;
      private readonly VkPostponePullerStrategy _postponePullerStrategy;
      private readonly HistoryPullerStrategy _historyPullerStrategy;

      public PullerStrategies(VkActualPullerStrategy actualPullerStrategy, VkPostponePullerStrategy postponePullerStrategy, HistoryPullerStrategy historyPullerStrategy) {
         _actualPullerStrategy = actualPullerStrategy;
         _postponePullerStrategy = postponePullerStrategy;
         _historyPullerStrategy = historyPullerStrategy;
      }

      public VkActualPullerStrategy ActualPullerStrategy => _actualPullerStrategy;
      public VkPostponePullerStrategy PostponePullerStrategy => _postponePullerStrategy;
      public HistoryPullerStrategy HistoryPullerStrategy => _historyPullerStrategy;
   }
}