using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class GroupsGet: IGroupsGet {
      private readonly VkApi _api;

      public GroupsGet(VkApi api) {
         _api = api;
      }

      public async Task<GroupsGetResponse> GetAsync(QueryParameters parameters, CancellationToken ct) {
         var query = buildAQuery();
         query.AppendParameters(parameters);

         var response = await _api.ExecuteMethodAsync("groups.get", query, ct).ConfigureAwait(false);
         return JsonConvert.DeserializeObject<GroupsGetResponse>(response);
      }

      public async Task<GroupsGetResponse> GetAsync(QueryParameters parameters) {
         return await GetAsync(parameters, CancellationToken.None).ConfigureAwait(false);
      }

      private static QueryParameters buildAQuery() {
         return QueryParameters.New()
            .AddParameter("extended", 1);
      }
   }

   [UsedImplicitly]
   public class GroupsGetResponse {
      [JsonProperty(PropertyName = "response")]
      public GroupGetCollection Content { get; set; }
   }

   [UsedImplicitly]
   public class GroupGetCollection {
      [JsonProperty(PropertyName = "count")]
      public int Count { get; set; }
      [JsonProperty(PropertyName = "items")]
      public List<Group> Groups { get; set; }
   }
}