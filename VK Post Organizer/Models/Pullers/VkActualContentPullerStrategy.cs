using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.Models.Pullers {
   public class VkActualTimePeriodContentPullerStrategy : IContentPullerStrategy {
      public VkActualTimePeriodContentPullerStrategy(WallGet wallGet, int days) {

      }

      public Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder, PullerSettings settings) {
         throw new System.NotImplementedException();
      }

      public Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder, PullerSettings settings, CancellationToken ct) {
         throw new System.NotImplementedException();
      }
   }

   [UsedImplicitly]
   public class VkActualContentPullerStrategy : IContentPullerStrategy {
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
         var postList = new List<IPost>();
         postList.AddRange(await getPostsWithAnOffset(wallHolder.ID, 100, 0, ct));
         postList.Sort((a, b) => a.Date.CompareTo(b.Date));
         return postList;
      }
   }
}