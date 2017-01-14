using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using vk.Models;
using vk.Utils;

namespace vk.ViewModels {
   public class SettingsViewModel : BindableBase {
      private Settings _currentSettings;
      private bool _isBusy;
      private string _proxyPingMs;

      public Settings CurrentSettings {
         get { return _currentSettings; }
         set { SetProperty(ref _currentSettings, value); }
      }

      public bool IsBusy {
         get { return _isBusy; }
         set { SetProperty(ref _isBusy, value); }
      }

      public string ProxyPingMs {
         get { return _proxyPingMs; }
         set { SetProperty(ref _proxyPingMs, value); }
      }

      public ICommand OkCommand { get; set; }
      public ICommand RevertSettingsCommand { get; set; }
      public ICommand PingCommand { get; set; }

      public SettingsViewModel() {
         OkCommand = new DelegateCommand<Window>(okCommandexecute);
         RevertSettingsCommand = new DelegateCommand(revert);

         PingCommand = new DelegateCommand(pingProxy);

         revert();
         //pingProxy();
      }

      private async void pingProxy() {
         if (CurrentSettings.Proxy.UseProxy) {
            if (UrlHelper.IsUrlIsValid(CurrentSettings.Proxy.ProxyAddress)) {
               try {
                  var ping = new Ping();
                  var reply = await ping.SendPingAsync(new Uri(CurrentSettings.Proxy.ProxyAddress).Host, 3000);
                  if (reply == null) return;
                  ProxyPingMs = $"~{reply.RoundtripTime} ms";
               }
               catch (PingException ex) {
                  MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", ex.ToString());
               }
            }

            var proxy = UrlHelper.GetProxy(CurrentSettings.Proxy.ProxyUri, CurrentSettings.Proxy.Username, CurrentSettings.Proxy.Password);
            if (proxy != null) {
               using (var wc = new WebClient()) {
                  wc.Encoding = Encoding.UTF8;
                  var uriBuilder = new UriBuilder($"https://api.vk.com/method/users.get?access_token=2312&v=5.60");

                  wc.Proxy = proxy;
                  try {
                     //MessageBox.Show("Downloading string via proxy, please wait...");
                     var str = await wc.DownloadStringTaskAsync(uriBuilder.Uri);
                     var some = str.Length;
                     //MessageBox.Show(str);
                  }
                  catch (Exception ex) {
                     MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", ex.ToString());
                  }
               }
            }
         }
      }

      private void revert() {
         CurrentSettings = new Settings(App.Container.Resolve<Settings>());
      }

      private async Task saveData() {
         if (CurrentSettings != null) {
            var settings = new Settings(CurrentSettings);

            App.Container.RegisterInstance(settings);
            await SaveLoaderHelper.SaveAsync("Settings", settings);
         }
      }

      private async void okCommandexecute(Window window) {
         IsBusy = true;
         await saveData();
         IsBusy = false;

         window?.Close();
      }
   }
}