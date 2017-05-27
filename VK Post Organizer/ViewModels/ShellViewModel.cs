using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using NLog;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Events;
using vk.Infrastructure;
using vk.Models;
using vk.Models.VkApi.Entities;
using vk.Views;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class ShellViewModel : BindableBase {
      private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
      private readonly IRegionManager _regionManager;
      private readonly IEventAggregator _eventAggregator;
      private readonly VkPostponeSaveLoader _saveLoader;
      private readonly Settings _settings;


      public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, VkPostponeSaveLoader saveLoader, 
         Settings settings) {
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
            logger.Debug($"Авторизация завершена успешно");
            _eventAggregator.GetEvent<WallSelectorEvents.FillWallRequest>().Publish();
         }
         else {
            logger.Debug($"Авторизация завершилась неудачей");
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.StartPage);
         }
      }

      private void onLogOutCompleted() {
         logger.Debug($"Пользователь вышел");
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

      public void OnLoad() {
         //todo get rid of silly workaround to preinitialize views
         _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.Content, result => {
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.AvailableWalls);
         });

         _eventAggregator.GetEvent<AuthBarEvents.AuthorizeIfAlreadyLoggined>().Publish();
         App.IsInitialized = true;
      }

      public void OnClosing() {
         _saveLoader.Save("Settings", _settings);
      }
   }
}