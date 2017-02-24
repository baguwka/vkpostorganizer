using System.Threading;
using System.Threading.Tasks;

namespace vk.Models.VkApi {
   public interface IUsersGet {
      Task<UsersGetResponse> GetAsync();
      Task<UsersGetResponse> GetAsync(CancellationToken ct);
   }
}