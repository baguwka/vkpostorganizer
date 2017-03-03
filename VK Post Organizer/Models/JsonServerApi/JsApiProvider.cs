using JetBrains.Annotations;

namespace vk.Models.JsonServerApi {
   [UsedImplicitly]
   public class JsApiProvider {
      public GetPosts GetPosts { get; private set; }

      public JsApiProvider(JsApi api) {
         GetPosts = new GetPosts(api);
      }
   }
}