using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models {
   [UsedImplicitly]
   public class HistoryPostViewModelBuilder : IPostViewModelBuilder {
      private readonly VkApiProvider _api;

      public HistoryPostViewModelBuilder(VkApiProvider api) {
         _api = api;
      }

      public IEnumerable<PostViewModelBase> Build(IEnumerable<IPost> posts) {
         var some = BuildAsync(posts);
         some.Wait();
         return some.Result;
      }

      public Task<IEnumerable<PostViewModelBase>> BuildAsync(IEnumerable<IPost> posts) {
         return BuildAsync(posts, CancellationToken.None);
      }

      public async Task<IEnumerable<PostViewModelBase>> BuildAsync([NotNull] IEnumerable<IPost> posts, CancellationToken ct) {
         if (posts == null) {
            throw new ArgumentNullException(nameof(posts));
         }

         var viewmodels = posts.Select(HistoryPostViewModel.Create).ToList();
         var publisherIds = viewmodels.Select(post => post.Post.OwnerId).Distinct().ToList();

         if (publisherIds.Any()) {
            var users = await _api.UsersGet.GetAsync(QueryParameters.New()
               .Add("user_ids", string.Join(",", publisherIds)), ct);

            if (users.Content.Any()) {
               foreach (var user in users.Content) {
                  var ownersPosts = viewmodels.Where(post => post.Post.OwnerId == user.ID);
                  foreach (var ownersPost in ownersPosts) {
                     ownersPost.PublisherName = user.Name;
                  }
               }
            }
         }
         return viewmodels;
      }
   }
}