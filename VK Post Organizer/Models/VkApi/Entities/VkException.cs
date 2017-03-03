using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace vk.Models.VkApi.Entities {
   public class VkException : Exception {
      public int ErrorCode { get; private set; }

      public VkException() {
      }

      public VkException(string message) : base(message) {
      }

      public VkException(string message, int errorCode) : base(message) {
         ErrorCode = errorCode;
      }

      public VkException(string message, Exception innerException) : base(message, innerException) {
      }

      public VkException(string message, Exception innerException, int errorCode) : base(message, innerException) {
         ErrorCode = errorCode;
      }

      protected VkException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) {
      }
   }
}