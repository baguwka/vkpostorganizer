using System;
using System.Collections.Generic;
using System.Linq;
using vk.Models.VkApi;

namespace vk.Utils {
   public static class GroupNameCache {
      public static Dictionary<int, string> GroupNamesById { get; set; }

      static GroupNameCache() {
         GroupNamesById = new Dictionary<int, string>();
      }

      public static string GetGroupName(int groupId) {
         if (GroupNamesById.ContainsKey(groupId)) {
            return GroupNamesById[groupId];
         }

         var groupsGet = App.Container.GetInstance<GroupsGetById>();
         var response = groupsGet.Get(Math.Abs(groupId));
         var group = response.Response.FirstOrDefault();

         GroupNamesById.Add(groupId, group?.Name);
         return group?.Name;
      }
   }
}