using System.Web;
using NUnit.Framework;
using vk.Models.VkApi;

namespace UnitTests.VkApi {
   [TestFixture]
   public class VkAuthorizationTesting {
      private const string EXAMPLE_TOKEN = "934jd91j29j89718237nfs8f72m348723mfsd87237324m";
      private const int EXAMPLE_USERID = 13474597;

      private string exampleUrlCorrectAnswer => $"https://oauth.vk.com/blank.html" +
                                         $"#access_token={EXAMPLE_TOKEN}&expires_in=0" +
                                         $"&user_id={EXAMPLE_USERID}";

      private string exampleUrlWrongAnswer => $"https://oauth.vk.com/blank.html" +
                                         $"#sdqqn={EXAMPLE_TOKEN}&expires_in=0" +
                                         $"&user_i0d={EXAMPLE_USERID}";


      [Test]
      public void Parse_the_url() {
         var auth = new VkAuthorization();
         var response = auth.ParseResultUrl(exampleUrlCorrectAnswer);
         Assert.That(response.Successful, Is.True);
         Assert.That(response.TokenResponse, Is.EqualTo(EXAMPLE_TOKEN));
         Assert.That(response.UserId, Is.EqualTo(EXAMPLE_USERID));
      }

      [Test]
      public void Parse_the_wrong_url() {
         var auth = new VkAuthorization();
         var response = auth.ParseResultUrl(exampleUrlWrongAnswer);
         Assert.That(response.Successful, Is.False);
      }

      [Test]
      public void Check_authorization_uri_parameters() {
         var uri = VkAuthorization.GetAuthorizationUri().Query;
         var query = HttpUtility.ParseQueryString(uri);

         Assert.That(query["response_type"], Is.EqualTo("token"));
         Assert.That(query["redirect_uri"], Is.EqualTo("oauth.vk.com/blank.html"));
         Assert.That(query["client_id"], Is.EqualTo(VkAuthorization.CLIENT_ID));
         Assert.That(query["scope"], Is.EqualTo(VkAuthorization.SCOPES));
         Assert.That(query["v"], Is.EqualTo(vk.Models.VkApi.VkApi.VERSION));
      }


      [Test]
      public void Setup_token_from_url() {
         var auth = new VkAuthorization();
         var token = new AccessToken();

         Assert.That(token.Token, Is.EqualTo(null).Or.EqualTo(string.Empty));
         Assert.That(token.UserID, Is.EqualTo(0));

         var response = auth.TryToSetupTokenFromUrl(token, exampleUrlCorrectAnswer);

         Assert.That(response.Successful, Is.True);
         Assert.That(token.Token, Is.EqualTo(EXAMPLE_TOKEN));
         Assert.That(token.UserID, Is.EqualTo(EXAMPLE_USERID));
      }
   }
}