using System.Threading;
using System.Threading.Tasks;

namespace vk.Models.VkApi {
   public interface IPhotosGetWallUploadSever {
      Task<UploadServerInfo> GetAsync(int groupId);
      Task<UploadServerInfo> GetAsync(int groupId, CancellationToken ct);
   }
}