using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnitTests.Fakes {
   public class FakeResponseHandlerByHost : DelegatingHandler {
      private readonly Dictionary<Uri, HttpResponseMessage> _fakeResponses = new Dictionary<Uri, HttpResponseMessage>();

      public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage) {
         _fakeResponses.Add(uri, responseMessage);
      }

      protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) {
         var firstMatch = _fakeResponses.Keys.FirstOrDefault(response => response.Host == request.RequestUri.Host);

         if (firstMatch == null) {
            return new HttpResponseMessage(HttpStatusCode.NotFound) {RequestMessage = request};
         }
         else {
            return _fakeResponses[firstMatch];
         }
      }
   }
}