using System.Threading;
using System.Threading.Tasks;

namespace vk.Models.VkApi {
   public interface IGroupsGet {
      Task<GroupsGetResponse> GetAsync(VkParameters parameters);
      Task<GroupsGetResponse> GetAsync(VkParameters parameters, CancellationToken ct);
   }
}