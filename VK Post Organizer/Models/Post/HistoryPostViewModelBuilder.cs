using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models {
   public class HistoryPostViewModelBuilder : IPostViewModelBuilder {
      public IEnumerable<PostViewModelBase> Build(IEnumerable<IPost> posts) {
         return posts.Select(HistoryPostViewModel.Create);
      }

      public Task<IEnumerable<PostViewModelBase>> BuildAsync(IEnumerable<IPost> posts) {
         return BuildAsync(posts, CancellationToken.None);
      }

      public Task<IEnumerable<PostViewModelBase>> BuildAsync(IEnumerable<IPost> posts, CancellationToken ct) {
         return Task.Run(() => Build(posts), ct);
      }
   }
}