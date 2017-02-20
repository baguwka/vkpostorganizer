﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Prism.Mvvm;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

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

      public async Task<DownloadPhotoInfo> DownloadPhotoByUriAsync(Uri uri, [NotNull] IProgress<int> progress, CancellationToken ct) {
         try {
            var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!response.IsSuccessStatusCode) {
               return new DownloadPhotoInfo {
                  Successful = false,
                  ErrorMessage = response.StatusCode.ToString() 
               };
            }

            return await readContentStreamAsync(progress, ct, response);
         }
         catch (OperationCanceledException ex) {
            return new DownloadPhotoInfo {
               Successful = false,
               Photo = null,
               ErrorMessage = ex.Message
            };
         }
      }

      /// <summary>
      /// Reads incoming content async with progress report.
      /// </summary>
      private static async Task<DownloadPhotoInfo> readContentStreamAsync(IProgress<int> progress, CancellationToken ct, HttpResponseMessage response) {
         if (response.Content.Headers.ContentLength == null) {
            return new DownloadPhotoInfo {
               Successful = false,
               ErrorMessage = "У загружаемого контента отсутствуют заголовки (Headers.ContentLenght == null)."
            };
         }

         var totalBytes = response.Content.Headers.ContentLength.Value;

         using (var contentStream = await response.Content.ReadAsStreamAsync()) {
            using (var stream = new MemoryStream(8192)) {
               var totalRead = 0L;
               var buffer = new byte[8192];
               var isMoreToRead = true;

               do {
                  if (ct.IsCancellationRequested) break;

                  var read = await contentStream.ReadAsync(buffer, 0, buffer.Length, ct);
                  if (read == 0) {
                     isMoreToRead = false;
                  }
                  else {
                     await stream.WriteAsync(buffer, 0, read, ct);

                     totalRead += read;

                     var progressPercent = (((float)totalRead / (float)totalBytes) * 100);
                     progress.Report((int)progressPercent);
                  }
               } while (isMoreToRead);

               var bytes = stream.ToArray();
               if (bytes.Length == 0) {
                  return new DownloadPhotoInfo {
                     Successful = false,
                     Photo = bytes,
                     ErrorMessage = "File size is 0 bytes"
                  };
               }

               return new DownloadPhotoInfo {
                  Successful = true,
                  Photo = bytes
               };
            }
         }
      }

      public async Task<UploadPhotoInfo> TryUploadPhotoToWallAsync(MultipartContent photo, int wallId, [NotNull] IProgress<int> progress, CancellationToken ct) {
         if (photo == null) throw new ArgumentNullException(nameof(photo));
         if (progress == null) throw new ArgumentNullException(nameof(progress));

         try {
            var uploadServer = await _getWallUploadServer.GetAsync(wallId);

            var response = await _client.PostAsync(new Uri(uploadServer.UploadUrl), photo, ct);

            if (!response.IsSuccessStatusCode || response.Content == null) {
               return new UploadPhotoInfo {
                  Successful = false,
                  ErrorMessage = "Метод POST не удался или Content == null"
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
         catch (VkException ex) {
            var errmessage = "";
            if (ex.ErrorCode == 100) {
               errmessage = "Не удалось загрузить данный файл как фото вк. Либо это не изображение, либо файл поврежден. Детали ниже:";
            }

            errmessage += ex.Message;

            return new UploadPhotoInfo {
               Successful = false,
               ErrorMessage = errmessage
            };
         }
         catch (OperationCanceledException ex) {
            return new UploadPhotoInfo {
               Successful = false,
               ErrorMessage = ex.Message
            };
         }
      }
   }

   public class DownloadPhotoInfo {
      public bool Successful { get; set; }
      public string ErrorMessage { get; set; }
      public byte[] Photo { get; set; }
   }
}