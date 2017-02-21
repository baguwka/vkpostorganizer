using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Data_Persistence_Provider;
using JetBrains.Annotations;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Events;
using vk.Infrastructure;
using vk.Models;
using vk.Models.VkApi.Entities;
using vk.Views;
using static System.Threading.Tasks.Task<string>;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class ShellViewModel : BindableBase {
      private readonly IRegionManager _regionManager;
      private readonly IEventAggregator _eventAggregator;
      private readonly VkPostponeSaveLoader _saveLoader;
      private readonly Settings _settings;

      public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, VkPostponeSaveLoader saveLoader, Settings settings) {
         _regionManager = regionManager;
         _eventAggregator = eventAggregator;
         _saveLoader = saveLoader;
         _settings = settings;

         _eventAggregator.GetEvent<VkAuthorizationEvents.AcquiredTheToken>().Subscribe((accessToken) => {
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.AvailableWalls);
         });

         _eventAggregator.GetEvent<WallSelectorEvents.WallSelected>().Subscribe(onWallItemClicked);

         _eventAggregator.GetEvent<MainBottomEvents.Back>().Subscribe(onBackRequested);
         _eventAggregator.GetEvent<MainBottomEvents.Settings>().Subscribe(onSettingsRequested);

         _eventAggregator.GetEvent<AuthBarEvents.AuthorizationCompleted>().Subscribe(onAthorizationCompleted);
         _eventAggregator.GetEvent<AuthBarEvents.LogOutCompleted>().Subscribe(onLogOutCompleted);

      }

      private void onAthorizationCompleted(bool result) {
         if (result) {
            _eventAggregator.GetEvent<WallSelectorEvents.FillWallRequest>().Publish();
         }
         else {
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.StartPage);
         }
      }

      private void onLogOutCompleted() {
         _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.StartPage);
      }

      private void onSettingsRequested() {
         var settings = new SettingsView();
         settings.ShowDialog();
      }

      private void onBackRequested() {
         _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.AvailableWalls);
         _eventAggregator.GetEvent<ShellEvents.WallSelectedEvent>().Publish(false);
         _eventAggregator.GetEvent<UploaderEvents.Configure>().Publish(new UploaderViewModelConfiguration {
            IsEnabled = false
         });
      }

      private void onWallItemClicked(WallItem wallItem) {
         try {
            //IsBusy = true;
            //await Wall.PullWithScheduleHightlightAsync(wallItem.WallHolder, CurrentPostTypeFilter.GetFilter(),
            //      CurrentSchedule);
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.Content, result => {
               _eventAggregator.GetEvent<UploaderEvents.Configure>().Publish(new UploaderViewModelConfiguration() {
                  WallId = wallItem.WallHolder.ID,
                  IsEnabled = true,
                  DateOverride = -1
               });
            });

            _eventAggregator.GetEvent<ShellEvents.WallSelectedEvent>().Publish(true);

         }
         catch (VkException ex) {
            MessageBox.Show(ex.Message, "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
         }
         finally {
            //IsBusy = false;
         }
      }

      public async void OnLoad() {
         //IsBusy = true;

         var mainVmData = await _saveLoader.TryLoadAsync<MainVMSaveInfo>("MainVM");
         if (mainVmData.Successful) {
            //TestingGroup = data.TestingGroup;
         }

         var loadedSettings = await _saveLoader.TryLoadAsync<Settings>("Settings");

         if (loadedSettings.Successful) {
            _settings.ApplySettings(loadedSettings.Result);
         }
         else {
            //defaults
            _settings.ApplySettings(new Settings());
         }

         //SetUpAvatar(DEFAULT_AVATAR);

         //await authorizeIfAlreadyLoggined();
         _eventAggregator.GetEvent<AuthBarEvents.AuthorizeIfAlreadyLoggined>().Publish();

         //IsBusy = false;

         //_eventAggregator.GetEvent<FillWallListEvent>().Publish();
      }

      public void OnClosing() {
         _saveLoader.Save("MainVM", new MainVMSaveInfo());
         _saveLoader.Save("Settings", _settings);
      }
   }

   [Serializable]
   public class MainVMSaveInfo : CommonSaveData {
      public MainVMSaveInfo() {
      }
   }
}