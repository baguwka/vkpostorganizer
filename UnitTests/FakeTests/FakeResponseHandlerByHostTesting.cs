using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using UnitTests.Fakes;

namespace UnitTests.FakeTests {
   [TestFixture]
   public class FakeResponseHandlerByHostTesting {
      [Test]
      public async Task FakeResponseByHost_pass_mathes_host_success_expected() {
         var handler = new FakeResponseHandlerByHost();

         var contentToCompare = "boop";

         handler.AddFakeResponse(new Uri("http://whatever.org/method=doesnotmatter?param=39&value=2"),
            new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(contentToCompare) });

         var httpClient = new HttpClient(handler);
         var result = await httpClient.GetAsync(new Uri("http://whatever.org/"));

         Assert.That(result.IsSuccessStatusCode, Is.True);

         var content = await result.Content.ReadAsStringAsync();

         Assert.That(content, Is.EqualTo(contentToCompare));
      }

      [Test]
      public async Task FakeResponseByHost_pass_invalid_host_fail_expected() {
         var handler = new FakeResponseHandlerByHost();

         var contentToCompare = "boop";

         handler.AddFakeResponse(new Uri("http://whatever.org/method=doesnotmatter?param=39&value=2"),
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(contentToCompare) });

         var httpClient = new HttpClient(handler);
         var result = await httpClient.GetAsync(new Uri("http://notwhatever.org/"));

         Assert.That(result.IsSuccessStatusCode, Is.False);
      }
   }
}