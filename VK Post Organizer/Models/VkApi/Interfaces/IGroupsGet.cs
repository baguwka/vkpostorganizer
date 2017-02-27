using System.Threading;
using System.Threading.Tasks;

namespace vk.Models.VkApi {
   public interface IGroupsGet {
      Task<GroupsGetResponse> GetAsync(QueryParameters parameters);
      Task<GroupsGetResponse> GetAsync(QueryParameters parameters, CancellationToken ct);
   }
}