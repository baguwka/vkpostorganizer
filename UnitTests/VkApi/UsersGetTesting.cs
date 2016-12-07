using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;
using vk.Models;
using vk.Models.VkApi;

namespace UnitTests.VkApi {
   [TestFixture]
   public class UsersGetTesting {

      [Test]
      public void pass_method_and_parameters_and_assert_that_data_is_correct() {
         //https://api.vk.com/method/{method}?{parameters}&access_token={_token.Token}&v=5.60

         string result = string.Empty;
         bool webClientCalled = false;

         var uc = new UnityContainer();

         var webClient = Substitute.For<IWebClient>();
         webClient.WhenForAnyArgs(x => x.DownloadString("")).Do(info => {
            result = info.ArgAt<string>(0);
            webClientCalled = true;
         });

         uc.RegisterInstance<IWebClient>(webClient);

         var usersGetMethod = uc.Resolve<UsersGet>();

         var usersInfo = usersGetMethod.Get();
      }

      private void configureContainer(UnityContainer uc) {
      }
   }
}
