using System;
using System.Net;
using System.Net.Http;
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
         var photo = new byte[1024 * 10];
         var contentToRecieve = new byte[1024 * 6];

         var fakeMessagehandler = new FakeResponseHandler();
         fakeMessagehandler.AddFakeResponse(new Uri("http://upload.uri/"), new HttpResponseMessage(HttpStatusCode.OK) {Content = new ByteArrayContent(contentToRecieve)});

         var uploader = new VkUploader(uploadServer, saveWallPhoto, fakeMessagehandler);
         var info = await uploader.UploadPhotoToWallAsync(photo, -1, progress, ct);

         Assert.That(info.Result?.Photo1280, Is.EqualTo(FakeResponsesForVkApi.ForPhotosSaveWallPhoto().Response[0]?.Photo1280));
         Assert.That(info.Successful, Is.True);
      }
   }
}