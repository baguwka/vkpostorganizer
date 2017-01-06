using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class PhotosGetWallUploadSever : VkApiBase {
      public PhotosGetWallUploadSever([NotNull] AccessToken token, [NotNull] IWebClient webClient) : base(token, webClient) {
      }

      public UploadServerInfo Get(int groupId) {
         groupId = Math.Abs(groupId);

         var response = ExecuteMethod("photos.getWallUploadServer", $"group_id={groupId}");

         checkForErrors(response);

         return JsonConvert.DeserializeObject<PhotosGetWallUploadServerResponse>(response)?.Response;
      }
   }

   [UsedImplicitly]
   public class PhotosGetWallUploadServerResponse {
      [JsonProperty(PropertyName = "response")]
      public UploadServerInfo Response { get; set; }
   }

   [UsedImplicitly]
   public class UploadServerInfo {
      [JsonProperty(PropertyName = "upload_url")]
      public string UploadUrl { get; set; }

      [JsonProperty(PropertyName = "album_id")]
      public int AlbumId { get; set; }

      [JsonProperty(PropertyName = "user_id")]
      public int UserId { get; set; }
   }


}