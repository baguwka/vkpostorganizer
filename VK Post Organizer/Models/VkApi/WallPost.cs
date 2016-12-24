using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class WallPost : VkApiBase {
      public WallPost([NotNull] AccessToken token, [NotNull] IWebClient webClient) : base(token, webClient) {
      }

      public WallPostResponse Post(int id, string message, bool signed, bool fromGroup, int date, IEnumerable<string> attachments = null) {
         var signedInt = signed ? 1 : 0;
         var fromGroupInt = fromGroup ? 1 : 0;
         if (message == null) {
            message = "";
         }

         var response = ExecuteMethod("wall.post", $"owner_id=-{id}&filter=postponed&publish_date={date}&signed={signedInt}&from_group={fromGroupInt}&message={message}");

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