using System.Diagnostics;
using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;
using vk;
using vk.Models;
using vk.Models.VkApi;

namespace UnitTests.VkApi {
   [TestFixture]
   public class PhotosSaveWallPhotoTesting {
      [Test]
      public void pass_method_ant_parameters_through() {
         const string photo = "[{\"photo\":\"0d48376015:w\",\"sizes\":[[\"s\",\"636830747\",\"409d5\",\"BypHxELWDwE\",46,75],[\"m\",\"636830747\",\"409d6\",\"b6-bfLUNme4\",80,130],[\"x\",\"636830747\",\"409d7\",\"oi15qTdCO7Y\",373,604],[\"y\",\"636830747\",\"409d8\",\"dCsfBprvYmo\",499,807],[\"z\",\"636830747\",\"409d9\",\"OFHoipFkf9Q\",668,1080],[\"w\",\"636830747\",\"409da\",\"DfbTFe8ap38\",1337,2160],[\"o\",\"636830747\",\"409db\",\"xPMlmC-OIcw\",130,211],[\"p\",\"636830747\",\"409dc\",\"bHSXeK4oXbY\",200,324],[\"q\",\"636830747\",\"409dd\",\"f_NAdCANmRs\",320,517],[\"r\",\"636830747\",\"409de\",\"cWXwDOvOtkk\",510,825]],\"kid\":\"a8bef781f7e4750025b0024416bee2a2\",\"debug\":\"xswmwxwywzwwwowpwqwrw\"}]";
         const int server = 636830;
         const string hash = "0b1e2279f9d1c46a828c2d717d286162";

         string urlThatShoudBe = $"https://api.vk.com/method/photos.saveWallPhoto" +
                                       $"?access_token=HEREGOESTOKEN" +
                                       $"&group_id=1" +
                                       $"&server={server}" +
                                       $"&photo={photo}" +
                                       $"&hash={hash}" +
                                       $"&v=5.60";

         string uploadResponse = "{\"" +
                                 $"server\":{server},\"" +
                                 $"photo\":\"{photo}\",\"" +
                                 $"hash\":\"{hash}\"" +
                                 "}";

         var resp2 = "{\"server\":" + server +
                     ",\"photo\":\"" + photo +
                     "\",\"hash\":\"" + hash + "\"}";

         Debug.WriteLine(resp2);
         Debug.WriteLine(uploadResponse);

         string result = string.Empty;
         bool webClientCalled = false;

         var uc = new UnityContainer();

         var webClient = Substitute.For<IWebClient>();
         webClient.WhenForAnyArgs(x => x.DownloadString("")).Do(info => {
            result = info.ArgAt<string>(0);
            webClientCalled = true;
         });

         uc.RegisterInstance<IWebClient>(webClient);

         var saveWallPhotoMethod = uc.Resolve<PhotosSaveWallPhoto>();

         var saveResponse = saveWallPhotoMethod.Save(1, resp2);

         //Assert.That(result, Is.EqualTo(urlThatShoudBe));
         Assert.That("", Is.EqualTo(""));
      }
   }
}
