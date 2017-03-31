using JetBrains.Annotations;

namespace vk.Models.Pullers {
   [UsedImplicitly]
   public class PullerStrategies {
      private readonly VkActualContentPullerStrategy _actualContentPullerStrategy;
      private readonly VkPostponedContentPullerStrategy _postponedContentPullerStrategy;
      private readonly HistoryContentPullerStrategy _historyContentPullerStrategy;

      public PullerStrategies(VkActualContentPullerStrategy actualContentPullerStrategy, 
         VkPostponedContentPullerStrategy postponedContentPullerStrategy, HistoryContentPullerStrategy historyContentPullerStrategy) {

         _actualContentPullerStrategy = actualContentPullerStrategy;
         _postponedContentPullerStrategy = postponedContentPullerStrategy;
         _historyContentPullerStrategy = historyContentPullerStrategy;
      }

      public VkActualContentPullerStrategy ActualContentPullerStrategy => _actualContentPullerStrategy;
      public VkPostponedContentPullerStrategy PostponedContentPullerStrategy => _postponedContentPullerStrategy;
      public HistoryContentPullerStrategy HistoryContentPullerStrategy => _historyContentPullerStrategy;
   }
}