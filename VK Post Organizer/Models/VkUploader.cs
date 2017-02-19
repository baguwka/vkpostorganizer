using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Prism.Mvvm;
using vk.Models.VkApi;

namespace vk.Models {
   public class VkUploader : BindableBase {
      private readonly IPhotosGetWallUploadSever _getWallUploadServer;
      private readonly IPhotosSaveWallPhoto _saveWallPhoto;
      private readonly HttpMessageHandler _messageHandler;
      private HttpClient _client;

      public VkUploader(IPhotosGetWallUploadSever getWallUploadServer, IPhotosSaveWallPhoto saveWallPhoto, HttpMessageHandler messageHandler) {
         _getWallUploadServer = getWallUploadServer;
         _saveWallPhoto = saveWallPhoto;
         _messageHandler = messageHandler;

         renewClient();
      }

      //todo: call it when proxy settings change
      private void renewClient() {
         _client = new HttpClient(_messageHandler);
      }

      public async Task<UploadPhotoInfo> UploadPhotoToWallAsync([NotNull] byte[] photo, int wallId, [NotNull] IProgress<int> progress, CancellationToken ct) {
         if (photo == null) throw new ArgumentNullException(nameof(photo));
         if (progress == null) throw new ArgumentNullException(nameof(progress));

         var uploadServer = await _getWallUploadServer.GetAsync(wallId);

         var byteContent = new ByteArrayContent(photo);
         var response = await _client.PostAsync(uploadServer.UploadUrl, byteContent, ct);

         if (!response.IsSuccessStatusCode) {
            return new UploadPhotoInfo {
               Successful = false
            };
         }

         if (response.Content == null) {
            return new UploadPhotoInfo {
               Successful = false
            };
         }

         var uploadResponse = await response.Content.ReadAsStringAsync();

         var savePhotoProperty = await _saveWallPhoto.SaveAsync(wallId, uploadResponse);
         var savedPhoto = savePhotoProperty?.Response.FirstOrDefault();

         return new UploadPhotoInfo {
            Result = savedPhoto,
            Successful = savedPhoto != null
         };
      }
   }
}