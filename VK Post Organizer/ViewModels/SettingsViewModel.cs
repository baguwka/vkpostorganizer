using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using vk.Models;
using vk.Utils;

namespace vk.ViewModels {
   public class PingInfo {
      public PingReply Reply { get; set; }
      public bool IsAvailable { get; set; }
   }

   public class SettingsViewModel : BindableBase {
      private readonly VkPostponeSaveLoader _saveLoader;
      private readonly Settings _settings;
      private bool _isBusy;
      private string _proxyPingMs;
      private string _historyServerPingMs;

      private Settings _currentSettings;

      public Settings CurrentSettings {
         get { return _currentSettings; }
         private set {
            if (_currentSettings != null) {
               _currentSettings.Proxy.PropertyChanged -= OnProxyPropertyChanged;
               _currentSettings.History.PropertyChanged -= OnHistoryPropertyChanged;
            }

            SetProperty(ref _currentSettings, value);

            if (_currentSettings != null) {
               _currentSettings.Proxy.PropertyChanged += OnProxyPropertyChanged;
               _currentSettings.History.PropertyChanged += OnHistoryPropertyChanged;
            }
         }
      }

      public bool IsBusy {
         get { return _isBusy; }
         set { SetProperty(ref _isBusy, value); }
      }

      public string ProxyPingMs {
         get { return _proxyPingMs; }
         set { SetProperty(ref _proxyPingMs, value); }
      }

      public string HistoryServerPingMs {
         get { return _historyServerPingMs; }
         set { SetProperty(ref _historyServerPingMs, value); }
      }

      public ICommand OkCommand { get; private set; }
      public ICommand RevertSettingsCommand { get; private set; }
      public DelegateCommand PingProxyCommand { get; private set; }
      public DelegateCommand PingHistoryServerCommand { get; private set; }

      public SettingsViewModel(VkPostponeSaveLoader saveLoader, Settings settings) {
         _saveLoader = saveLoader;
         _settings = settings;
         OkCommand = new DelegateCommand<Window>(okCommandexecute);
         RevertSettingsCommand = new DelegateCommand(revert);

         CurrentSettings = new Settings();

         PingProxyCommand = new DelegateCommand(pingProxy,
               () => CurrentSettings.Proxy.UseProxy && UrlHelper.IsUrlIsValid(CurrentSettings.Proxy.ProxyAddress));

         PingHistoryServerCommand = new DelegateCommand(pingHistoryServerCommandExecute,
               () => CurrentSettings.History.Use && UrlHelper.IsUrlIsValid(CurrentSettings.History.Url));

         revert();
      }

      private async void pingHistoryServerCommandExecute() {
         var response = await checkAvailability(CurrentSettings.History.Uri);
         if (!response.IsAvailable) {
            HistoryServerPingMs = "n/a";
            return;
         }
         HistoryServerPingMs = $"~{response.Reply?.RoundtripTime} ms";
      }

      private async void pingProxy() {
         var response = await checkAvailability(CurrentSettings.Proxy.ProxyUri);
         if (!response.IsAvailable) {
            ProxyPingMs = "n/a";
            return;
         }
         ProxyPingMs = $"~{response.Reply?.RoundtripTime} ms";
      }

      private static async Task<PingInfo> checkAvailability(Uri uri) {
         try {
            var ping = new Ping();
            var pingReply = await ping.SendPingAsync(uri.Host, 3000);

            //check port
            using (var client = new TcpClient()) {
               await client.ConnectAsync(uri.Host, uri.Port);
               return new PingInfo {
                  Reply = pingReply,
                  IsAvailable = true
               };
            }
         }
         catch (PingException) {
            return new PingInfo {IsAvailable = false};
         }
         catch (Exception) {
            return new PingInfo { IsAvailable = false };
         }
      }

      private void OnHistoryPropertyChanged(object sender, PropertyChangedEventArgs e) {
         PingHistoryServerCommand.RaiseCanExecuteChanged();
      }

      private void OnProxyPropertyChanged(object sender, PropertyChangedEventArgs e) {
         PingProxyCommand.RaiseCanExecuteChanged();
      }

      private void revert() {
         CurrentSettings.ApplySettings(_settings);
      }

      private async Task saveData() {
         if (CurrentSettings != null) {
            _settings.ApplySettings(CurrentSettings);
            await _saveLoader.SaveAsync("Settings", _settings);
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