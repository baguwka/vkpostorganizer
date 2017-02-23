using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using vk.Models.VkApi;

namespace vk.Utils {
   public static class GroupNameCache {
      private static ConcurrentDictionary<int, string> _groupNamesById { get; }

      static GroupNameCache() {
         _groupNamesById = new ConcurrentDictionary<int, string>();
      }

      public static async Task<string> GetGroupNameAsync(int groupId) {
         if (_groupNamesById.ContainsKey(groupId)) {
            return _groupNamesById[groupId];
         }

         var groupsGetById = App.Container.Resolve<GroupsGetById>();

         var response = await groupsGetById.GetAsync(groupId);
         var group = response.Response.FirstOrDefault();

         _groupNamesById.TryAdd(groupId, group?.Name);
         return group?.Name;
      }
   }
}