using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace vk.Models.JsonServerApi {
   public class JsonServerException : Exception {
      public JsonServerException() {
      }

      public JsonServerException(string message) : base(message) {
      }

      public JsonServerException(string message, Exception innerException) : base(message, innerException) {
      }

      protected JsonServerException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) {
      }
   }
}