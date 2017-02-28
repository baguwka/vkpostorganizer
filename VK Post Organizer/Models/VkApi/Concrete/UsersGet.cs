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

      public Task<UsersGetResponse> GetAsync(QueryParameters query) {
         return GetAsync(query, CancellationToken.None);
      }

      public async Task<UsersGetResponse> GetAsync(QueryParameters query, CancellationToken ct) {
         var response = await _api.ExecuteMethodAsync("users.get", query, ct).ConfigureAwait(false);
         return JsonConvert.DeserializeObject<UsersGetResponse>(response);
      }

      //private static QueryParameters buildAQuery() {
      //   var query = QueryParameters.New()
      //      .Add("fields", "first_name,last_name,photo_50");
      //   return query;
      //}
   }

   [UsedImplicitly]
   public class UsersGetResponse {
      [JsonProperty(PropertyName = "response")]
      public List<User> Content { get; set; }
   }
}
