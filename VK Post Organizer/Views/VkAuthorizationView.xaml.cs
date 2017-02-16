using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using vk.ViewModels;

namespace vk.Views {
   [Obsolete]
   /// <summary>
   /// Interaction logic for AhuthorizationView.xaml
   /// </summary>
   public partial class VkAuthorizationView : UserControl {
      private readonly AuthViewModel _viewModel;

      public VkAuthorizationView() {
         InitializeComponent();
         HideScriptErrors(InternalWebBrowser, true);

         _viewModel = (AuthViewModel)DataContext;
         Loaded += onViewLoaded;
      }

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
         //InternalWebBrowser.MessageHook += internalWebBrowserOnMessageHook;
         //_viewModel.OnLoaded();
         //InternalWebBrowser.Navigate(destinationUrl);
      }

      private void WebBrowser_OnNavigated(object sender, NavigationEventArgs e) {
         var url = e.Uri.Fragment;
         //_viewModel.OnWebBrowserNavigated(url);
      }

      //private IntPtr internalWebBrowserOnMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
      //   //msg = 130 is the last call for when the window gets closed on a window.close() in javascript
      //   if (msg == 130) {
      //      _viewModel.Close();
      //   }
      //   return IntPtr.Zero;
      //}
   }
}
