using JetBrains.Annotations;
using vk.Models.JsonServerApi;
using vk.Models.VkApi;

namespace vk.Models.Pullers {
   [UsedImplicitly]
   public class PullerStrategies {
      private readonly VkActualContentPullerStrategy _actualContentPullerStrategy;
      private readonly VkPostponedContentPullerStrategy _postponedContentPullerStrategy;
      private readonly HistoryContentPullerStrategy _historyContentPullerStrategy;
      private readonly VkActualTimePeriodContentPullerStrategy _actualTimePeriodContentPullerStrategy;


      public PullerStrategies(VkApiProvider vkApi, JsApiProvider jsApi) {
         _actualContentPullerStrategy = new VkActualContentPullerStrategy(vkApi.WallGet);
         _actualTimePeriodContentPullerStrategy = new VkActualTimePeriodContentPullerStrategy(vkApi.WallGet, 3);
         _postponedContentPullerStrategy = new VkPostponedContentPullerStrategy(vkApi.WallGet);
         _historyContentPullerStrategy = new HistoryContentPullerStrategy(jsApi.GetPosts);
      }

      public VkActualContentPullerStrategy ActualContentPullerStrategy => _actualContentPullerStrategy;
      public VkPostponedContentPullerStrategy PostponedContentPullerStrategy => _postponedContentPullerStrategy;
      public HistoryContentPullerStrategy HistoryContentPullerStrategy => _historyContentPullerStrategy;
      public VkActualTimePeriodContentPullerStrategy ActualTimePeriodContentPullerStrategy => _actualTimePeriodContentPullerStrategy;
   }
}