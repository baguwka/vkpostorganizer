using System.Threading.Tasks;
using NUnit.Framework;
using UnitTests.Fakes;
using vk.Models;
using vk.Models.JsonServerApi;
using vk.Models.Pullers;

namespace UnitTests.Puller {
   [TestFixture]
   public class HistoryContentPullerTesting {
      private JsApiProvider _apiProvider;

      [SetUp]
      public void Setup() {
         var handler = new FakeResponseHandler();
         var historysettings = new HistorySettings { Use = true, Url = "whatever.com", Port = 3000 };
         var jsApi = new JsApi(handler, historysettings);
         _apiProvider = new JsApiProvider(jsApi);
      }

      [Test]
      public async Task History_puller_pull_async() {
         IContentPullerStrategy contentPullerStrategy = new HistoryContentPullerStrategy(_apiProvider.GetPosts);
         IContentPuller contentPuller = new ContentPuller(contentPullerStrategy);

         contentPuller.WallHolder = new WallHolder(1);
         await contentPuller.PullAsync();

         Assert.Fail();
      }
   }
}