using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using vk.Models.JsonServerApi;
using vk.Models.VkApi.Entities;

namespace vk.Models.Pullers {
   public class HistoryContentPullerStrategy : IContentPullerStrategy {
      private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
      public const int ITEMS_PER_PAGE = 50;

      private readonly GetPosts _getPosts;

      public HistoryContentPullerStrategy(GetPosts getPosts) {
         _getPosts = getPosts;
      }

      private async Task<IEnumerable<IPost>> getPostsOfPage(int wallId, int count, int page) {
         try {
            logger.Debug($"«апрос на пулл контента с JsonServer дл€ стены #{wallId} со страницы #{page} в количестве {count}");
            var response = await _getPosts.GetAsync(wallId, count, page);
            return response.Content.ToList();
         }
         catch (JsonServerException ex) {
            logger.Error(ex, $"ѕроизошла ошибка во врем€ выполнени€ пулла контента с JsonServer дл€ стены #{wallId} со страницы #{page} в количестве {count}");
            //ignore (???) todo: do not ignore
         }
         return null;
      }

      public Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder, PullerSettings settings) {
         return GetAsync(wallHolder, settings, CancellationToken.None);
      }

      public async Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder, PullerSettings settings, CancellationToken ct) {
         var postList = new List<IPost>();
         postList.AddRange(await getPostsOfPage(wallHolder.ID, ITEMS_PER_PAGE, 0));
         postList.Sort((a, b) => b.Date.CompareTo(a.Date));
         return postList;
      }
   }
}