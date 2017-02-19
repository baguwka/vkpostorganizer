using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class PhotosSaveWallPhoto : VkApiBase, IPhotosSaveWallPhoto {
      public PhotosSaveWallPhoto([NotNull] AccessToken token, [NotNull] IWebClient webClient) : base(token, webClient) {
      }

      [Obsolete]
      public PhotosSaveWallPhotoResponse Save(int groupID, string uploadResponse) {
         var query = buildAQuery(groupID, uploadResponse);
         var response = ExecuteMethod("photos.saveWallPhoto", query);
         return JsonConvert.DeserializeObject<PhotosSaveWallPhotoResponse>(response);
      }

      public async Task<PhotosSaveWallPhotoResponse> SaveAsync(int groupID, string uploadResponse) {
         var query = buildAQuery(groupID, uploadResponse);
         var response = await ExecuteMethodAsync("photos.saveWallPhoto", query);
         return JsonConvert.DeserializeObject<PhotosSaveWallPhotoResponse>(response);
      }

      private static VkParameters buildAQuery(int groupID, string uploadResponse) {
         groupID = Math.Abs(groupID);

         var jsonedResponse = JsonConvert.DeserializeObject(uploadResponse) as JObject;

         if (jsonedResponse == null) {
            throw new VkException("cannot deserialize upload response");
         }

         var jserver = jsonedResponse["server"];
         var jphoto = jsonedResponse["photo"];
         var jhash = jsonedResponse["hash"];
         var query = VkParameters.New()
            .AddParameter("group_id", groupID)
            .AddParameter("server", jserver)
            .AddParameter("photo", jphoto)
            .AddParameter("hash", jhash);
         return query;
      }
   }

   [UsedImplicitly]
   public class PhotosSaveWallPhotoResponse {
      [JsonProperty(PropertyName = "response")]
      public List<Photo> Response { get; set; }
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
      [CanBeNull]
      public Photo Result { get;  set; }
      public bool Successful { get;  set; }
   }
}