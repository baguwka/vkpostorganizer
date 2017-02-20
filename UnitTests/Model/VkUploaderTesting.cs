using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using UnitTests.Fakes;
using vk.Models;

namespace UnitTests.Model {
   [TestFixture]
   public class VkUploaderTesting {
      [Test]
      public async Task Upload_photo_to_server_and_verify_progress() {
         var progress = new Progress<int>();
         var uploadServer = SubstituresForVkApi.PhotosGetWallUploadSever();
         var saveWallPhoto = SubstituresForVkApi.PhotosSaveWallPhoto();
         var ct = CancellationToken.None;
         var photo = new byte[1024 * 1];
         var contentToRecieve = new byte[1024 * 2];

         var fakeMessagehandler = new FakeResponseHandler();
         fakeMessagehandler.AddFakeResponse(new Uri("http://upload.uri/"), new HttpResponseMessage(HttpStatusCode.OK) {Content = new ByteArrayContent(contentToRecieve)});

         var uploader = new VkUploader(uploadServer, saveWallPhoto, fakeMessagehandler);

         var requestContent = new MultipartFormDataContent();
         var imageContent = new ByteArrayContent(photo);
         imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
         requestContent.Add(imageContent, "image", "image.jpg");

         var info = await uploader.TryUploadPhotoToWallAsync(requestContent, -1, progress, ct);

         Assert.That(info.Result?.Photo1280, Is.EqualTo(FakeResponsesForVkApi.ForPhotosSaveWallPhoto().Response[0]?.Photo1280));
         Assert.That(info.Successful, Is.True);
      }
   }
}