using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.Models.Pullers {
   [UsedImplicitly]
   public class VkActualContentPullerStrategy : IContentPullerStrategy {
      private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

      private readonly WallGet _wallGet;

      public VkActualContentPullerStrategy(WallGet wallGet) {
         _wallGet = wallGet;
      }

      private async Task<IEnumerable<IPost>> getPostsWithAnOffset(int wallHolderId, int count, int offset, CancellationToken ct) {
         try {
            var query = QueryParameters.New()
               .Add("owner_id", wallHolderId)
               .Add("offset", offset)
               .Add("count", count);

            logger.Debug($"Получение {count} постовс актуальной стены {wallHolderId} со смещением {offset}");

            var response = await _wallGet.GetAsync(query, ct);
            var posts = response.Content.Wall.ToList();

            logger.Debug($"Посты актуальной стены #{wallHolderId} успешно получены. Всего их {posts.Count} (ожидалось {count})");
            return posts;
         }
         catch (VkException ex) {
            logger.Error(ex, $"Произошла ошибка во время получения постов актуальной стены #{wallHolderId}");
            throw;
         }
      }

      public Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder, PullerSettings settings) {
         return GetAsync(wallHolder, settings, CancellationToken.None);
      }

      public async Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder, PullerSettings settings, CancellationToken ct) {
         var postList = new List<IPost>();
         postList.AddRange(await getPostsWithAnOffset(wallHolder.ID, 100, 0, ct));
         postList.Sort((a, b) => a.Date.CompareTo(b.Date));
         return postList;
      }
   }
}