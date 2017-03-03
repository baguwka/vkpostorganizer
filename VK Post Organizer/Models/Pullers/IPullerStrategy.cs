using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vk.Models.VkApi.Entities;

namespace vk.Models.Pullers {
   public interface IPullerStrategy {
      Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder);
      Task<IEnumerable<IPost>> GetAsync(IWallHolder wallHolder, CancellationToken ct);
   }
}