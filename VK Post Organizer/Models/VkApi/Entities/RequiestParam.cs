using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class RequiestParam {
      [JsonProperty(PropertyName = "key")]
      public string Key { get; set; }

      [JsonProperty(PropertyName = "value")]
      public string Value { get; set; }
   }
}