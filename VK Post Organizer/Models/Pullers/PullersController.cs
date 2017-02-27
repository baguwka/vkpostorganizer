using System.Threading.Tasks;
using JetBrains.Annotations;
using Prism.Events;
using vk.Events;
using vk.Models.Filter;

namespace vk.Models.Pullers {
   [UsedImplicitly]
   public class PullersController {
      private readonly IEventAggregator _eventAggregator;
      private IWallHolder _sharedWallHolder;
      public VkWallPuller Vk { get; }
      public HistoryPuller History { get; }

      public IWallHolder SharedWallHolder {
         get { return _sharedWallHolder; }
         set {
            _sharedWallHolder = value;
            Vk.WallHolder = _sharedWallHolder;
            History.WallHolder = _sharedWallHolder;
         }
      }

      public PullersController(IEventAggregator eventAggregator, VkWallPuller vkWallVk, HistoryPuller historyPuller) {
         _eventAggregator = eventAggregator;

         Vk = vkWallVk;
         History = historyPuller;

         _eventAggregator.GetEvent<MainBottomEvents.Refresh>().Subscribe(async () => {
            await SharedPull().ConfigureAwait(false);
         });
      }

      public async Task SharedPull() {
         var vkRefresh = Vk.PullWithScheduleHightlightAsync(new NoPostFilter(), new Schedule());
         var historyRefresh = History.PullAsync();
         await Task.WhenAll(vkRefresh, historyRefresh).ConfigureAwait(false);
      }
   }
}