using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace vk.Models.VkApi.Entities {
   [UsedImplicitly]
   public class Error {
      [JsonProperty(PropertyName = "error_code")]
      public int ErrorCode { get; set; }

      [JsonProperty(PropertyName = "error_msg")]
      public string ErrorMessage { get; set; }

      [JsonProperty(PropertyName = "request_params")]
      public List<RequiestParam> RequiestParams { get; set; }
   }
}