using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models {
   public interface IPostViewModelBuilder {
      IEnumerable<PostViewModelBase> Build(IEnumerable<IPost> posts);

      Task<IEnumerable<PostViewModelBase>> BuildAsync(IEnumerable<IPost> posts);
      Task<IEnumerable<PostViewModelBase>> BuildAsync(IEnumerable<IPost> posts, CancellationToken ct);
   }
}