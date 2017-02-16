using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Awesomium.Core;
using vk.ViewModels;
using NavigationEventArgs = System.Windows.Navigation.NavigationEventArgs;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for AuthView.xaml
   /// </summary>
   public partial class AuthView : Window {
      private readonly AuthViewModel _viewModel;

      public AuthView() {
         InitializeComponent();
         HideScriptErrors(InternalWebBrowser, true);

         _viewModel = (AuthViewModel)DataContext;
         Loaded += onViewLoaded;
         Closing += (sender, args) => {
            InternalWebBrowser.MessageHook -= internalWebBrowserOnMessageHook;
         };
      }

      public AuthAction Action { get; set; }

      //private void onAdressChanged(object sender, UrlEventArgs e) {
      //   var url = e.Url.ToString();
      //   //close if token acquired
      //   if (_viewModel.OnWebBrowserNavigated(url)) {
      //      this.Close();
      //   }
      //}

      public static void HideScriptErrors(WebBrowser wb, bool hide) {
         var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
         if (fiComWebBrowser == null) return;
         var objComWebBrowser = fiComWebBrowser.GetValue(wb);
         if (objComWebBrowser == null) {
            wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
            return;
         }
         objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
      }

      private void onViewLoaded(object sender, RoutedEventArgs routedEventArgs) {
         InternalWebBrowser.MessageHook += internalWebBrowserOnMessageHook;
         _viewModel.OnLoaded(Action);
         //InternalWebBrowser.Navigate(destinationUrl);
      }

      private void WebBrowser_OnNavigated(object sender, NavigationEventArgs e) {
         Debug.WriteLine("ON NAVIGATED");
         var url = e.Uri.ToString();
         //close if token acquired
         if (_viewModel.OnWebBrowserNavigated(url, Action)) {
            this.Close();
         }
      }

      private IntPtr internalWebBrowserOnMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
         //msg = 130 is the last call for when the window gets closed on a window.close() in javascript
         if (msg == 130) {
            this.Close();
         }
         return IntPtr.Zero;
      }

   }
}
