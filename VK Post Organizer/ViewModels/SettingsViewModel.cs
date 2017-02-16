using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using vk.Models;
using vk.Utils;

namespace vk.ViewModels {
   public class SettingsViewModel : BindableBase {
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

      public SettingsViewModel() {
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
         var response = await ping(CurrentSettings.History.Url);
         HistoryServerPingMs = $"~{response?.RoundtripTime} ms";
      }

      private async void pingProxy() {
         var response = await ping(CurrentSettings.Proxy.ProxyAddress);
         ProxyPingMs = $"~{response?.RoundtripTime} ms";
      }

      private async Task<PingReply> ping(string url) {
         try {
            var ping = new Ping();
            return await ping.SendPingAsync(new Uri(url).Host, 3000);
         }
         catch (PingException ex) {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", ex.ToString());
         }
         return null;
      }

      private void OnHistoryPropertyChanged(object sender, PropertyChangedEventArgs e) {
         PingHistoryServerCommand.RaiseCanExecuteChanged();
      }

      private void OnProxyPropertyChanged(object sender, PropertyChangedEventArgs e) {
         PingProxyCommand.RaiseCanExecuteChanged();
      }

      private void revert() {
         CurrentSettings.ApplySettings(App.Container.GetInstance<Settings>());
      }

      private async Task saveData() {
         if (CurrentSettings != null) {
            var settings = App.Container.GetInstance<Settings>();
            settings.ApplySettings(CurrentSettings);

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