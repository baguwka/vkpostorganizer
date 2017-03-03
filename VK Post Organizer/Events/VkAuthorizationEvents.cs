using Prism.Events;
using vk.Models.VkApi;

namespace vk.Events {
   public class VkAuthorizationEvents {
      public class AcquiredTheToken : PubSubEvent<AccessToken> { }
   }
}