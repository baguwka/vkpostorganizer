using System;
using System.Collections.Specialized;
using JetBrains.Annotations;

namespace vk.Models.VkApi {
   public class VkParameters {
      public NameValueCollection Query { get; }

      public static VkParameters No() {
         return new VkParameters();
      }

      public static VkParameters New() {
         return new VkParameters();
      }

      public string this[string key] => Query[key];

      public VkParameters() {
         Query = new NameValueCollection();
      }

      public VkParameters AddParameter(string paramName, string paramValue) {
         if (!string.IsNullOrEmpty(paramValue)) {
            Query[paramName] = paramValue;
         }
         return this;
      }

      public VkParameters AddParameter(string paramName, object paramValue) {
         return AddParameter(paramName, paramValue.ToString());
      }

      public VkParameters AddParameters([NotNull] VkParameters parameters) {
         if (parameters == null) {
            throw new ArgumentNullException(nameof(parameters));
         }

         Query.Add(parameters.Query);
         return this;
      }
   }
}