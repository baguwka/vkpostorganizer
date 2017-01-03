using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using vk.Models.VkApi.Entities;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class PhotosSaveWallPhoto : VkApiBase {
      public PhotosSaveWallPhoto([NotNull] AccessToken token, [NotNull] IWebClient webClient) : base(token, webClient) {
      }

      public PhotosSaveWallPhotoResponse Save(int group_id, string uploadUrl, string imagePath) {
         group_id = Math.Abs(group_id);
         var uploadResponse = Encoding.UTF8.GetString(webClient.UploadFile(uploadUrl, "POST", imagePath));
         var jsonedResponse = JsonConvert.DeserializeObject(uploadResponse) as JObject;

         if (jsonedResponse == null) {
            throw new VkException("cannot deserialize upload response");
         }

         var jserver = jsonedResponse["server"];
         var jphoto = jsonedResponse["photo"];
         var jhash = jsonedResponse["hash"];

         var response = ExecuteMethod("photos.saveWallPhoto", $"group_id={group_id}" +
                                                              $"&server={jserver}" +
                                                              $"&photo={jphoto}" +
                                                              $"&hash={jhash}");

         checkForErrors(response);

         return JsonConvert.DeserializeObject<PhotosSaveWallPhotoResponse>(response);
      }
   }

   [UsedImplicitly]
   public class PhotosSaveWallPhotoResponse {
      [JsonProperty(PropertyName = "response")]
      public List<PhotosSaveWallPhotoInfo> Response { get; set; }
   }

   [UsedImplicitly]
   public class PhotosSaveWallPhotoInfo {
      [JsonProperty(PropertyName = "owner_id")]
      public int OwnerId { get; set; }

      [JsonProperty(PropertyName = "id")]
      public int Id { get; set; }
   }
}