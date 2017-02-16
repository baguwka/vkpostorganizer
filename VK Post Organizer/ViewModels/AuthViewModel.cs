using System.Diagnostics;
using JetBrains.Annotations;
using Prism.Events;
using Prism.Mvvm;
using vk.Events;
using vk.Models;
using vk.Models.VkApi;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class AuthViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;
      private readonly AccessToken _token;
      private readonly VkAuthorization _vkAuthorization;
      private string _webAdress;

      public string WebAdress {
         get { return _webAdress; }
         set { SetProperty(ref _webAdress, value); }
      }

      public AuthViewModel(IEventAggregator eventAggregator, AccessToken token, VkAuthorization vkAuthorization) {
         _eventAggregator = eventAggregator;
         _token = token;
         _vkAuthorization = vkAuthorization;
      }

      public bool OnWebBrowserNavigated(string url, AuthAction authAction) {
         Debug.WriteLine($"Browser navigated to url = {url}\nAction = {authAction}");
         switch (authAction) {
            case AuthAction.Authorize:
               var response = _vkAuthorization.TryToSetupTokenFromUrl(_token, url);
               if (response.Successful) {
                  _eventAggregator.GetEvent<VkAuthorizationEvents.AcquiredTheToken>().Publish(_token);
                  return true;
               }
               return false;

            case AuthAction.Deauthorize:
               return true;
         }
         return false;
      }

      public void OnLoaded(AuthAction authAction) {
         switch (authAction) {
            case AuthAction.Authorize:
               NavigateToAuthorizationPage();
               break;
            case AuthAction.Deauthorize:
               NavigateToDeauthorizationPage();
               break;
         }
      }

      private void NavigateToDeauthorizationPage() {
         var destionationUrl = VkAuthorization.GetLogoutUri();
         WebAdress = destionationUrl.ToString();
      }

      private void NavigateToAuthorizationPage() {
         var destinationUrl = VkAuthorization.GetAuthorizationUri();
         WebAdress = destinationUrl.ToString();
      }
   }
}