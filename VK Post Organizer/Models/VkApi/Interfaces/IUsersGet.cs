using System.Threading;
using System.Threading.Tasks;

namespace vk.Models.VkApi {
   public interface IUsersGet {
      Task<UsersGetResponse> GetAsync(QueryParameters query);
      Task<UsersGetResponse> GetAsync(QueryParameters query, CancellationToken ct);
   }
}