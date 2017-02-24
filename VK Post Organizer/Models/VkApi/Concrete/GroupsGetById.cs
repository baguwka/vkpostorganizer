using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class GroupsGetById {
      private readonly VkApi _api;

      public GroupsGetById(VkApi api) {
         _api = api;
      }

      public async Task<GroupsGetByIdResponse> GetAsync(int id, CancellationToken ct) {
         var query = buildAQuery(id);
         var response = await _api.ExecuteMethodAsync("groups.getById", query, ct).ConfigureAwait(false);
         return JsonConvert.DeserializeObject<GroupsGetByIdResponse>(response);
      }

      public async Task<GroupsGetByIdResponse> GetAsync(int id) {
         return await GetAsync(id, CancellationToken.None).ConfigureAwait(false);
      }

      private static VkParameters buildAQuery(int id) {
         id = Math.Abs(id);
         var query = VkParameters.New()
            .AddParameter("group_id", id);
            //.AddParameter("fields", fields);
         return query;
      }
   }

   [UsedImplicitly]
   public class GroupsGetByIdResponse {
      [JsonProperty(PropertyName = "response")]
      public List<GroupGetByIdItem> Response { get; set; }
   }

   [UsedImplicitly]
   public class GroupGetByIdItem {
      [JsonProperty(PropertyName = "id")]
      public int ID { get; set; }

      [JsonProperty(PropertyName = "name")]
      public string Name { get; set; }

      [JsonProperty(PropertyName = "is_admin")]
      public int IsAdmin { get; set; }

      [JsonProperty(PropertyName = "is_member")]
      public int IsMember{ get; set; }

      [JsonProperty(PropertyName = "description")]
      public string Description { get; set; }

      [JsonProperty(PropertyName = "photo_50")]
      public string Photo50 { get; set; }

      [JsonProperty(PropertyName = "photo_100")]
      public string Photo100 { get; set; }

      [JsonProperty(PropertyName = "photo_200")]
      public string Photo200 { get; set; }
   }
}