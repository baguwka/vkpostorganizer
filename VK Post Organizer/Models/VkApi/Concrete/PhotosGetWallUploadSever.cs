using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class PhotosGetWallUploadSever: IPhotosGetWallUploadSever {
      private readonly VkApiBase _api;

      public PhotosGetWallUploadSever(VkApiBase api) {
         this._api = api;
      }

      public async Task<UploadServerInfo> GetAsync(int groupId) {
         groupId = Math.Abs(groupId);
         var response = await _api.ExecuteMethodAsync("photos.getWallUploadServer", VkParameters.New().AddParameter("group_id", groupId));
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