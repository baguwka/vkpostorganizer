using System.Collections.Generic;
using vk.Models.VkApi.Entities;

namespace vk.Models.Pullers {
   public class ContentPullerEventArgs {
      public IList<IPost> Items { get; set; }
      public bool Successful { get; set; }
      public bool Error { get; set; }
      public VkException VkException { get; set; }
   }
}