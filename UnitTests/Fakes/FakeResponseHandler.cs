using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnitTests.Fakes {
   public class FakeResponseHandler : DelegatingHandler {
      private readonly Dictionary<Uri, HttpResponseMessage> _fakeResponses = new Dictionary<Uri, HttpResponseMessage>();

      public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage) {
         _fakeResponses.Add(uri, responseMessage);
      }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
      protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) {
         if (_fakeResponses.ContainsKey(request.RequestUri)) {
            return _fakeResponses[request.RequestUri];
         }
         else {
            return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
         }
      }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
   }
}