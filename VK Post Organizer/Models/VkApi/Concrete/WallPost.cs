using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class WallPost {
      private readonly VkApi _api;
      
      public WallPost(VkApi api) {
         this._api = api;
      }

      public async Task<WallPostResponse> PostAsync(int wallId, string message, bool signed, bool fromGroup, int date, IEnumerable<string> attachments = null) {
         var parameters = makeAQuery(wallId, message, signed, fromGroup, date, attachments);
         return await PostAsync(parameters, CancellationToken.None);
      }

      public async Task<WallPostResponse> PostAsync(VkParameters parameters, CancellationToken ct) {
         var response = await _api.ExecuteMethodAsync("wall.post", parameters, ct).ConfigureAwait(false);
         return JsonConvert.DeserializeObject<WallPostResponse>(response);
      }

      private static VkParameters makeAQuery(int wallId, string message, bool signed, bool fromGroup, int date,
         IEnumerable<string> attachments) {
         var signedInt = signed ? 1 : 0;
         var fromGroupInt = fromGroup ? 1 : 0;
         if (message == null) {
            message = "";
         }

         var attachmentsString = "";

         if (attachments != null) {
            attachmentsString = string.Join(",", attachments);
         }
         var parameters = VkParameters.New()
            .AddParameter("owner_id", wallId)
            .AddParameter("filter", "postponed")
            .AddParameter("publish_date", date)
            .AddParameter("signed", signedInt)
            .AddParameter("from_group", fromGroupInt)
            .AddParameter("attachments", attachmentsString);

         parameters.AddParameter("message", message);
         return parameters;
      }

   }

   [UsedImplicitly]
   public class WallPostResponse {
      [JsonProperty(PropertyName = "response")]
      public WallPostInfo Response { get; set; }
   }

   [UsedImplicitly]
   public class WallPostInfo {
      [JsonProperty(PropertyName = "post_id")]
      public int PostID { get; set; }
   }
}