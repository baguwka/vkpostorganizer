using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using UnitTests.Fakes;
using UnitTests.Properties;
using vk.Models;
using vk.Models.Pullers;
using vk.Models.VkApi;

namespace UnitTests.Puller {
   [TestFixture]
   public class VkContentPullerTesting {
      private VkApiProvider _apiProvider;

      [SetUp]
      public void Setup() {
         var token = new AccessToken();
         var handler = new FakeResponseHandlerByHost();
         handler.AddFakeResponse(new Uri($"https://api.vk.com/method/wall.get?"),
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Resources.FakeWallGetResponse) });

         var vkApi = new vk.Models.VkApi.VkApi(token, handler);
         _apiProvider = new VkApiProvider(token, vkApi);
      }

      [Test]
      public async Task Vk_timePeriod_puller_pull() {
         IContentPullerStrategy contentPullerStrategy = new VkActualTimePeriodContentPullerStrategy(_apiProvider.WallGet);
         IContentPuller contentPuller = new ContentPuller(contentPullerStrategy);

         await contentPuller.PullAsync(new TimePeriodSettings {Days = 5});
      }

      [Test]
      public async Task Vk_puller_pull_async() {
         IContentPullerStrategy contentPullerStrategy = new VkPostponedContentPullerStrategy(_apiProvider.WallGet);
         IContentPuller contentPuller = new ContentPuller(contentPullerStrategy);

         var event_pullInvoked_called = false;
         var event_pullCompleted_called = false;

         contentPuller.PullInvoked += (sender, args) => {
            event_pullInvoked_called = true;
         };

         contentPuller.PullCompleted += (sender, args) => {
            event_pullCompleted_called = true;
         };

         contentPuller.WallHolder = new WallHolder(1);
         await contentPuller.PullAsync();

         Assert.That(event_pullInvoked_called, Is.True);
         Assert.That(event_pullCompleted_called, Is.True);
         Assert.That(contentPuller.Items.Count, Is.EqualTo(4));
         Assert.That(contentPuller.LastTimePulled, Is.LessThanOrEqualTo(DateTimeOffset.Now));
      }

   }
}