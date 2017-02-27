using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using vk.Models.History;
using vk.Models.VkApi;

namespace vk.Models.JsonServerApi {
   public class GetPosts {
      private readonly JsApi _api;

      public GetPosts(JsApi api) {
         _api = api;
      }

      public async Task<GetPostsResponse> GetAsync(int wallId, int count, int page = 1) {
         var response = await _api.CallAsync("posts", QueryParameters.New()
            .AddParameter("wall_id", wallId)
            .AddParameter("_page", page)
            .AddParameter("_limit", count), CancellationToken.None);

         var content = JsonConvert.DeserializeObject<IList<HistoryPost>>(response);
         return new GetPostsResponse() {Content = content};
      }
   }

   [Serializable]
   public class GetPostsResponse {
      public IList<HistoryPost> Content { get; set; }
   }
}