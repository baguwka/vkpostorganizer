using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {

   [UsedImplicitly]
   public class UsersGet : IUsersGet {
      private readonly VkApi _api;

      public UsersGet(VkApi api) {
         this._api = api;
      }

      public async Task<UsersGetResponse> GetAsync(CancellationToken ct) {
         var query = buildAQuery();
         var response = await _api.ExecuteMethodAsync("users.get", query, ct).ConfigureAwait(false);
         return JsonConvert.DeserializeObject<UsersGetResponse>(response);
      }

      public async Task<UsersGetResponse> GetAsync() {
         return await GetAsync(CancellationToken.None);
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
