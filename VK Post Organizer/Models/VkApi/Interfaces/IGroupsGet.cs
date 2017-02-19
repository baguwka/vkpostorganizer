using System.Threading.Tasks;

namespace vk.Models.VkApi {
   public interface IGroupsGet {
      Task<GroupsGetResponse> GetAsync(VkParameters parameters);
   }
}