using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using UnitTests.Fakes;

namespace UnitTests.FakeTests {
   [TestFixture]
   public class FakeResponseHandlerTesting {
      [Test]
      public async Task FakeResponse_pass_mathes_url_success_expected() {
         var handler = new FakeResponseHandler();

         var contentToCompare = "boop";

         handler.AddFakeResponse(new Uri("http://whatever.org/method=doesnotmatter?param=39&value=2"),
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(contentToCompare) });

         var httpClient = new HttpClient(handler);
         var result = await httpClient.GetAsync(new Uri("http://whatever.org/method=doesnotmatter?param=39&value=2"));

         Assert.That(result.IsSuccessStatusCode, Is.True);

         var content = await result.Content.ReadAsStringAsync();

         Assert.That(content, Is.EqualTo(contentToCompare));
      }

      [Test]
      public async Task FakeResponse_pass_invalid_url_fail_expected() {
         var handler = new FakeResponseHandler();

         var contentToCompare = "boop";

         handler.AddFakeResponse(new Uri("http://whatever.org/method=doesnotmatter?param=39&value=2"),
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(contentToCompare) });

         var httpClient = new HttpClient(handler);
         var result = await httpClient.GetAsync(new Uri("http://whatever.org/method=doesnotmatter?param=38&value=2"));

         Assert.That(result.IsSuccessStatusCode, Is.False);
      }
   }
}