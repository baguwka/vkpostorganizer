using System;
using System.Collections.Specialized;
using JetBrains.Annotations;

namespace vk.Models.VkApi {
   public class QueryParameters {
      public NameValueCollection Query { get; }

      public static QueryParameters No() {
         return new QueryParameters();
      }

      public static QueryParameters New() {
         return new QueryParameters();
      }

      public string this[string key] => Query[key];

      public QueryParameters() {
         Query = new NameValueCollection();
      }

      public QueryParameters AddParameter(string paramName, string paramValue) {
         if (!string.IsNullOrEmpty(paramValue)) {
            Query[paramName] = paramValue;
         }
         return this;
      }

      public QueryParameters AddParameter(string paramName, object paramValue) {
         return AddParameter(paramName, paramValue.ToString());
      }

      public QueryParameters AppendParameters([NotNull] QueryParameters parameters) {
         if (parameters == null) {
            throw new ArgumentNullException(nameof(parameters));
         }

         Query.Add(parameters.Query);
         return this;
      }
   }
}