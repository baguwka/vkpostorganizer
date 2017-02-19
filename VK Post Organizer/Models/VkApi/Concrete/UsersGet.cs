using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {

   [UsedImplicitly]
   public class UsersGet : VkApiBase, IUsersGet {
      public UsersGet(AccessToken token, IWebClient webClient) : base(token, webClient) {
      }

      [Obsolete]
      public UsersGetResponse Get() {
         var query = buildAQuery();
         var response = ExecuteMethod("users.get", query);
         return JsonConvert.DeserializeObject<UsersGetResponse>(response);
      }

      public async Task<UsersGetResponse> GetAsync() {
         var query = buildAQuery();
         var response = await ExecuteMethodAsync("users.get", query);
         return JsonConvert.DeserializeObject<UsersGetResponse>(response);
      }

      private static VkParameters buildAQuery() {
         var query = VkParameters.New()
            .AddParameter("fields", "first_name,last_name,photo_50");
         return query;
      }
   }

   [UsedImplicitly]
   public class UsersGetResponse {
      [JsonProperty(PropertyName = "response")]
      public List<User> Users { get; set; }
   }
}
