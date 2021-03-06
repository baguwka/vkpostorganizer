using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class PhotosSaveWallPhoto : IPhotosSaveWallPhoto {
      private readonly VkApi _api;

      public PhotosSaveWallPhoto(VkApi api) {
         this._api = api;
      }

      public async Task<PhotosSaveWallPhotoResponse> SaveAsync(int groupID, string uploadResponse, CancellationToken ct) {
         var query = buildAQuery(groupID, uploadResponse);
         var response = await _api.ExecuteMethodAsync("photos.saveWallPhoto", query, ct).ConfigureAwait(false);
         return JsonConvert.DeserializeObject<PhotosSaveWallPhotoResponse>(response);
      }

      private static QueryParameters buildAQuery(int groupID, string uploadResponse) {
         groupID = Math.Abs(groupID);

         var jsonedResponse = JsonConvert.DeserializeObject(uploadResponse) as JObject;

         if (jsonedResponse == null) {
            throw new VkException("cannot deserialize upload response");
         }

         var jserver = jsonedResponse["server"];
         var jphoto = jsonedResponse["photo"];
         var jhash = jsonedResponse["hash"];
         var query = QueryParameters.New()
            .Add("group_id", groupID)
            .Add("server", jserver)
            .Add("photo", jphoto)
            .Add("hash", jhash);
         return query;
      }

      public Task<PhotosSaveWallPhotoResponse> SaveAsync(int groupID, string uploadResponse) {
         return SaveAsync(groupID, uploadResponse, CancellationToken.None);
      }
   }

   [UsedImplicitly]
   public class PhotosSaveWallPhotoResponse {
      [JsonProperty(PropertyName = "response")]
      public List<Photo> Content { get; set; }
   }

   [UsedImplicitly]
   public class PhotosSaveWallPhotoInfo {
      [JsonProperty(PropertyName = "owner_id")]
      public int OwnerId { get; set; }

      [JsonProperty(PropertyName = "id")]
      public int Id { get; set; }

      [JsonProperty(PropertyName = "photo_75")]
      public int Photo75 { get; set; }

      [JsonProperty(PropertyName = "photo_130")]
      public int Photo130 { get; set; }

      [JsonProperty(PropertyName = "photo_604")]
      public int Photo604 { get; set; }

      [JsonProperty(PropertyName = "photo_807")]
      public int Photo807 { get; set; }

      [JsonProperty(PropertyName = "photo_1280")]
      public int Photo1280 { get; set; }

      [JsonProperty(PropertyName = "photo_2560")]
      public int Photo2560 { get; set; }

   }

   public class UploadPhotoInfo {
      public Photo Photo { get;  set; }
      public bool Successful { get;  set; }
      public string ErrorMessage { get; set; }
   }
}