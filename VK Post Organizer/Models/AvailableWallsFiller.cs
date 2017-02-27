using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using vk.Models.VkApi;

namespace vk.Models {
   [UsedImplicitly]
   public class AvailableWallsFiller {
      private readonly IUsersGet _usersGet;
      private readonly IGroupsGet _groupsGet;

      public AvailableWallsFiller(IUsersGet usersGet, IGroupsGet groupsGet) {
         _usersGet = usersGet;
         _groupsGet = groupsGet;
      }

      public async Task<AvailableWallsInfo> FillAsync(WallList wallList) {
         var groups = await _groupsGet.GetAsync(QueryParameters.New()
            .AddParameter("filter", "editor")
            .AddParameter("fields", "description"));

         if (groups.Content == null) {
            return new AvailableWallsInfo {
               Succeed = false,
               ErrorMessage = "Groups not found"
            };
         }

         var users = await _usersGet.GetAsync();
         if (users.Users == null) {
            return new AvailableWallsInfo {
               Succeed = false,
               ErrorMessage = "No users found at all"
            };
         }

         var user = users.Users.FirstOrDefault();
         if (user == null) {
            return new AvailableWallsInfo {
               Succeed = false,
               ErrorMessage = "User not found"
            };
         }

         //todo: get rid of workaround
         var item = new WallItem(new EmptyWallHolder {
            Name = user.Name,
            ID = Math.Abs(user.ID),
            Description = user.Description,
            Photo200 = user.Photo200,
            Photo50 = user.Photo50
         });

         wallList.Clear();
         wallList.Add(item);

         foreach (var group in groups.Content.Groups) {
            group.ID = -group.ID;
            wallList.Add(new WallItem(group));
         }

         return new AvailableWallsInfo {
            Succeed = true
         };
      }
   }

   public class AvailableWallsInfo {
      public bool Succeed { get; set; }
      public string ErrorMessage { get; set; }
   }
}