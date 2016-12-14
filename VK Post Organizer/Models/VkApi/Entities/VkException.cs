using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace vk.Models.VkApi.Entities {
   public class VkException : Exception {
      public VkException() {
      }

      public VkException(string message) : base(message) {
      }

      public VkException(string message, Exception innerException) : base(message, innerException) {
      }

      protected VkException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) {
      }
   }
}