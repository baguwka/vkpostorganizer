using System.Collections.Generic;
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
         var tasks = posts.Select(async post => {
            var vm = VkPostViewModel.Create(post);
            vm.IsExisting = true;
            if (vm.PostType == PostType.Repost) {
               var name = "";
               //user owns this post
               if (vm.Post.OwnerId > 0) {
                  var result = await _api.UsersGet.GetAsync(QueryParameters.New()
                     .Add("fields", "first_name,last_name")
                     .Add("user_ids", post.OwnerId), ct);

                  var user = result?.Content?.FirstOrDefault();
                  if (user != null) {
                     name = user.Name;
                  }
               }

               //group owns this post
               if (vm.Post.OwnerId < 0) {
                  var result = await _api.GroupsGetById.GetAsync(vm.Post.OwnerId, ct);
                  var group = result?.Content?.FirstOrDefault();
                  if (group != null) {
                     name = group.Name;
                  }
               }

               vm.Post.Message = $"{name.Substring(0, name.Length > 10 ? 10 : name.Length)}... {vm.Post.Message}";
            }
            return vm;
         });

         var vms = await Task.WhenAll(tasks);
         return vms;
      }
   }
}