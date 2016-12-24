using System;
using System.Windows;
using System.Windows.Navigation;
using JetBrains.Annotations;
using vk.Models.VkApi;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for AuthView.xaml
   /// </summary>
   public partial class AuthView : Window {
      private readonly AccessToken _token;

      private const string SCOPES = "offline,wall,groups";
      private const string CLIENT_ID = "5730368";

      public AuthView([NotNull] AccessToken token, bool clearCookies) {
         InitializeComponent();
         _token = token;

         Loaded += (sender, args) => {
            InternalWebBrowser.MessageHook += internalWebBrowserOnMessageHook;

            if (clearCookies) {
               deleteCookies();
            }

            var destinationUrl =
               $"https://oauth.vk.com/" +
               $"authorize?client_id={CLIENT_ID}&response_type=token" +
               $"&v=5.60&scope={SCOPES}&redirect_uri=oauth.vk.com/blank.html";
            InternalWebBrowser.Navigate(destinationUrl);
         };

         Closing += (sender, args) => InternalWebBrowser.MessageHook -= internalWebBrowserOnMessageHook;
      }

      private static void deleteCookies() {
         var expiration = DateTime.UtcNow - TimeSpan.FromDays(1);
         string cookie = $"remixsid=; expires={expiration.ToString("R")}; path=/; domain=.vk.com";
         Application.SetCookie(new Uri("https://www.vk.com"), cookie);
      }

      private IntPtr internalWebBrowserOnMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
         //msg = 130 is the last call for when the window gets closed on a window.close() in javascript
         if (msg == 130) {
            this.Close();
         }
         return IntPtr.Zero;
      }

      private void WebBrowser_OnNavigated(object sender, NavigationEventArgs e) {
         var url = e.Uri.Fragment;
         if (url.Contains("access_token") && url.Contains("#")) {
            var response = url.Split('=', '&');
            _token.Token = response[1];
            //_token.UserID = int.Parse(response[5]);
            DialogResult = true;
            this.Close();
         }
      }

      private void WebBrowser_OnNavigating(object sender, NavigatingCancelEventArgs e) {
      }
   }
}
