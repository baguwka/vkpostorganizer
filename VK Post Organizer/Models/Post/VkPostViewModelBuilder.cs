using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.ViewModels;

namespace vk.Models {
   public class VkPostViewModelBuilder : IPostViewModelBuilder {
      private readonly VkApiProvider _api;

      public VkPostViewModelBuilder(VkApiProvider api) {
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

      public async Task<IEnumerable<PostViewModelBase>> BuildAsync(IEnumerable<IPost> posts, CancellationToken ct) {
         var vms = posts.Select(post => {
            var vm = VkPostViewModel.Create(post);
            vm.IsExisting = true;
            return vm;
         }).ToList();

         //await GetOwnerNamesOfPosts(vms.Where(p => p.PostType == PostType.Repost), ct);

         return vms;
      }

      public Task GetOwnerNamesOfPosts(IEnumerable<VkPostViewModel> posts) {
         return GetOwnerNamesOfPosts(posts, CancellationToken.None);
      }

      public async Task GetOwnerNamesOfPosts(IEnumerable<VkPostViewModel> posts, CancellationToken ct) {
         var vkPostViewModels = posts as IList<VkPostViewModel> ?? posts.ToList();

         var publisherIds = vkPostViewModels.Select(post => post.Post.OwnerId).Distinct().ToList();

         var userOwners = publisherIds.Where(id => id > 0).ToList();
         var groupOwners = publisherIds.Where(id => id < 0).Select(Math.Abs).ToList();

         if (userOwners.Any()) {
            var users = await _api.UsersGet.GetAsync(QueryParameters.New()
               .Add("user_ids", string.Join(",", userOwners)), ct);

            if (users.Content.Any()) {
               foreach (var user in users.Content) {
                  var ownersPosts = vkPostViewModels.Where(post => post.Post.OwnerId == user.ID);
                  foreach (var ownersPost in ownersPosts) {
                     ownersPost.Post.Message = $"{user.Name.Substring(0, user.Name.Length > 20 ? 20 : user.Name.Length)}... {ownersPost.Post.Message}";
                  }
               }
            }
         }

         if (groupOwners.Any()) {
            var groups = await _api.GroupsGetById.GetAsync(QueryParameters.New()
               .Add("group_ids", string.Join(",", groupOwners)), ct);

            if (groups.Content.Any()) {
               foreach (var group in groups.Content) {
                  var ownersPosts = vkPostViewModels.Where(post => Math.Abs(post.Post.OwnerId) == group.ID);
                  foreach (var ownersPost in ownersPosts) {
                     ownersPost.Post.Message = $"{group.Name.Substring(0, group.Name.Length > 20 ? 20 : group.Name.Length)}... {ownersPost.Post.Message}";
                  }
               }
            }
         }
      }
   }
}