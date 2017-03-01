using System.Threading.Tasks;
using NUnit.Framework;
using UnitTests.Fakes;
using vk.Models;
using vk.Models.JsonServerApi;
using vk.Models.Pullers;
using vk.Models.VkApi;

namespace UnitTests.Puller {
   [TestFixture]
   public class ContentPullerTesting {
      [Test]
      public async Task Vk_puller_pull_async() {
         var token = new AccessToken();
         var handler = new FakeResponseHandler();
         var vkApi = new vk.Models.VkApi.VkApi(token, handler);
         var apiProvider = new VkApiProvider(token, vkApi);

         IPullerStrategy pullerStrategy = new VkPostponePullerStrategy(apiProvider.WallGet);
         IContentPuller contentPuller = new ContentPuller(pullerStrategy);

         contentPuller.WallHolder = new WallHolder(1);
         await contentPuller.PullAsync();
      }

      [Test]
      public async Task History_puller_pull_async() {
         var handler = new FakeResponseHandler();
         var historysettings = new HistorySettings {Use = true, Url = "whatever.com", Port = 3000};
         var jsApi = new JsApi(handler, historysettings);
         var jsApiProvider = new JsApiProvider(jsApi);

         IPullerStrategy pullerStrategy = new HistoryPullerStrategy(jsApiProvider.GetPosts);
         IContentPuller contentPuller = new ContentPuller(pullerStrategy);

         contentPuller.WallHolder = new WallHolder(1);
         await contentPuller.PullAsync();
      }
   }
}