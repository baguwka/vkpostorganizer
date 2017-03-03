using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using NUnit.Framework;
using vk.Models.VkApi;

namespace UnitTests.VkApi {
   [TestFixture]
   public class QueryParametersTesting {
      [Test]
      public void Fill_parameters_and_check_them() {
         var parameters = QueryParameters.New()
            .Add("param1", "value1")
            .Add("param2", "value2")
            .Add("param3", "value3")
            .Add("param4", "value4");

         Assert.That(parameters["param1"], Is.EqualTo("value1"));
         Assert.That(parameters["param2"], Is.EqualTo("value2"));
         Assert.That(parameters["param3"], Is.EqualTo("value3"));
         Assert.That(parameters["param4"], Is.EqualTo("value4"));
      }

      [Test]
      public void Add_parameters_to_parameters_and_check_them() {
         var parameters = QueryParameters.New()
            .Add("param1", "value1")
            .Add("param2", "value2");

         var externalParameters = QueryParameters.New()
            .Add("externalParam1", "externalValue1")
            .Add("externalParam2", "externalValue2");

         parameters.Append(externalParameters);

         Assert.That(parameters["param1"], Is.EqualTo("value1"));
         Assert.That(parameters["param2"], Is.EqualTo("value2"));
         Assert.That(parameters["externalParam1"], Is.EqualTo("externalValue1"));
         Assert.That(parameters["externalParam2"], Is.EqualTo("externalValue2"));
      }

      [Test]
      public void Build_a_uri_from_parameters() {
         var parameters = QueryParameters.New()
            .Add("param1", "value1")
            .Add("param2", "value2");

         var uriBuilder = new UriBuilder($"https://api.vk.com/method/nomethod");
         var uriParameters = new NameValueCollection();

         if (parameters != null) {
            uriParameters.Add(parameters.Query);
         }

         uriParameters["access_token"] = "token";
         uriParameters["v"] = "version";

         uriBuilder.Query = string.Join("&", uriParameters.AllKeys
            .Select(key => $"{key}={HttpUtility.UrlEncode(uriParameters[key])}"));

         var uri = uriBuilder.Uri;

         var proofUri = new Uri("https://api.vk.com/method/nomethod" +
                                "?param1=value1" +
                                "&param2=value2" +
                                "&access_token=token" +
                                "&v=version");

         Assert.That(uri, Is.EqualTo(proofUri));
      }
   }
}