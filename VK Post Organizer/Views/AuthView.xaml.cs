using System;
using System.Windows;
using System.Windows.Navigation;
using JetBrains.Annotations;
using vk.Models;
using vk.Models.VkApi;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for AuthView.xaml
   /// </summary>
   public partial class AuthView : Window {
      private readonly AccessToken _token;

      public AuthView([NotNull] AccessToken token) {
         _token = token;
         InitializeComponent();
         Loaded += (sender, args) => {
            InternalWebBrowser.MessageHook += internalWebBrowserOnMessageHook;
            deleteCookies();
            var destinationURL =
               $"https://oauth.vk.com/authorize?client_id=5730368&display=page&response_type=token&v=5.60&redirect_uri=oauth.vk.com/blank.html";
            InternalWebBrowser.Navigate(destinationURL);
         };
      }

      private static void deleteCookies() {
         string cookie = $"c_user=; expires={DateTime.UtcNow.AddDays(-1).ToString("R"):R}; path=/; domain=.vk.com";
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
            _token.UserID = Int32.Parse(response[5]);
            DialogResult = true;
            this.Close();
         }
      }

      private void WebBrowser_OnNavigating(object sender, NavigatingCancelEventArgs e) {
      }
   }
}
