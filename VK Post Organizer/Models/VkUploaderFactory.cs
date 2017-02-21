//using System.Net.Http;
//using JetBrains.Annotations;
//using vk.Models.VkApi;

//namespace vk.Models {
//   [UsedImplicitly]
//   public class VkUploaderFactory {
//      private readonly IPhotosGetWallUploadSever _getWallUploadServer;
//      private readonly IPhotosSaveWallPhoto _saveWallPhoto;
//      private readonly ProxyProvider _proxyProvider;

//      public VkUploaderFactory(IPhotosGetWallUploadSever getWallUploadServer, IPhotosSaveWallPhoto saveWallPhoto, ProxyProvider proxyProvider) {
//         _getWallUploadServer = getWallUploadServer;
//         _saveWallPhoto = saveWallPhoto;
//         _proxyProvider = proxyProvider;
//      }

//      public VkUploader BuildNewVkUploader() {
//         var proxy = _proxyProvider.GetProxy();

//         var handler = new HttpClientHandler {
//            Proxy = proxy,
//            PreAuthenticate = true,
//            UseDefaultCredentials = false
//         };

//         return new VkUploader(_getWallUploadServer, _saveWallPhoto, handler);
//      }
//   }
//}