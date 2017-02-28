using JetBrains.Annotations;

namespace vk.Models.Pullers {
   [UsedImplicitly]
   public class PullerStrategies {
      private readonly VkActualPullerStrategy _actualPullerStrategyStrategy;
      private readonly VkPostponePullerStrategy _postponePullerStrategyStrategy;
      private readonly HistoryPullerStrategy _historyPullerStrategyStrategy;

      public PullerStrategies(VkActualPullerStrategy actualPullerStrategyStrategy, VkPostponePullerStrategy postponePullerStrategyStrategy, HistoryPullerStrategy historyPullerStrategyStrategy) {
         _actualPullerStrategyStrategy = actualPullerStrategyStrategy;
         _postponePullerStrategyStrategy = postponePullerStrategyStrategy;
         _historyPullerStrategyStrategy = historyPullerStrategyStrategy;
      }

      public VkActualPullerStrategy ActualPullerStrategyStrategy => _actualPullerStrategyStrategy;
      public VkPostponePullerStrategy PostponePullerStrategyStrategy => _postponePullerStrategyStrategy;
      public HistoryPullerStrategy HistoryPullerStrategyStrategy => _historyPullerStrategyStrategy;
   }
}