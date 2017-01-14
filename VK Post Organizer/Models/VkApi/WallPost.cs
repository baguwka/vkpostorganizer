using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class WallPost : VkApiBase {
      public WallPost([NotNull] AccessToken token, [NotNull] IWebClient webClient) : base(token, webClient) {
      }

      protected string addParam(string paramName, string paramValue) {
         return !string.IsNullOrEmpty(paramValue) ? $"&{paramName}={paramValue}" : string.Empty;
      }

      protected string addParam(string paramName, int paramValue) {
         return addParam(paramName, paramValue.ToString());
      }

      public WallPostResponse Post(int wallId, string message, bool signed, bool fromGroup, int date, IEnumerable<string> attachments = null) {
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
         var response = ExecuteMethod("wall.post", parameters);

         checkForErrors(response);

         return JsonConvert.DeserializeObject<WallPostResponse>(response);
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