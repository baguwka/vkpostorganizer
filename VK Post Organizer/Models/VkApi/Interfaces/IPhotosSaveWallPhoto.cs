using System.Threading;
using System.Threading.Tasks;

namespace vk.Models.VkApi {
   public interface IPhotosSaveWallPhoto {
      Task<PhotosSaveWallPhotoResponse> SaveAsync(int groupID, string uploadResponse, CancellationToken ct);
      Task<PhotosSaveWallPhotoResponse> SaveAsync(int groupID, string uploadResponse);
   }
}