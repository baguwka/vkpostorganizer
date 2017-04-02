using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.Models.Pullers {
   public class VkActualTimePeriodContentPullerStrategy : IContentPullerStrategy {
      private readonly WallGet _wallGet;

      public VkActualTimePeriodContentPullerStrategy(WallGet wallGet) {
         _wallGet = wallGet;
      }

      private async Task<IEnumerable<IPost>> getPostsWithAnOffset(int wallHolderId, int count, int offset, CancellationToken ct) {
         try {
            var query = QueryParameters.New()
               .Add("owner_id", wallHolderId)
               .Add("filter", "postponed")
               .Add("offset", offset)
               .Add("count", count);

            var response = await _wallGet.GetAsync(query, ct);
            return response.Content.Wall.ToList();
         }
         catch (VkException ex) {
            throw;
         }
      }

      public Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder, PullerSettings settings) {
         return GetAsync(wallHolder, settings, CancellationToken.None);
      }

      public async Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder, PullerSettings settings, CancellationToken ct) {
         var timePeriodSettings = settings as TimePeriodSettings;
         if (timePeriodSettings == null) {
            var noSettings = settings as NoPullerSettings;
            if (noSettings == null) {
               throw new VkException("Не предоставлены корректные настройки");
            }
         }

         var days = timePeriodSettings?.Days ?? 1;

         var postList = new List<IPost>();

         const int postsToPull = 25;
         int offset = 0;
         while (true) {
            postList.AddRange(await getPostsWithAnOffset(wallHolder.ID, postsToPull, offset, ct));
            offset += postsToPull;
            if (postList.LastOrDefault().Date > days) ;
         }

         if (postList.Count == 100) {
            postList.AddRange(await getPostsWithAnOffset(wallHolder.ID, 50, 100, ct));
         }
         postList.Sort((a, b) => a.Date.CompareTo(b.Date));
         return postList;
      }
   }
}