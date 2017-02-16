using System.Windows;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Events;
using vk.Infrastructure;
using vk.Models;
using vk.Models.VkApi.Entities;
using vk.Views;

namespace vk.ViewModels {
   public class ShellViewModel : BindableBase {
      private readonly IRegionManager _regionManager;
      private readonly IEventAggregator _eventAggregator;

      public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) {
         _regionManager = regionManager;
         _eventAggregator = eventAggregator;

         _eventAggregator.GetEvent<VkAuthorizationEvents.AcquiredTheToken>().Subscribe((accessToken) => {
            _regionManager.RequestNavigate(RegionNames.MainRegion, "AvailableWalls");
         });

         _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(StartPageView));
         _regionManager.RegisterViewWithRegion(RegionNames.BottomRegion, typeof(MainBottomView));
         _regionManager.RegisterViewWithRegion(RegionNames.AuthRegion, typeof(AuthBarView));

         _eventAggregator.GetEvent<WallSelectorEvents.WallSelected>().Subscribe(onWallItemClicked);

         _eventAggregator.GetEvent<MainBottomEvents.Back>().Subscribe(onBackRequested);
         _eventAggregator.GetEvent<MainBottomEvents.Settings>().Subscribe(onSettingsRequested);

         _eventAggregator.GetEvent<AuthBarEvents.AuthorizationCompleted>().Subscribe(onAthorizationCompleted);
         _eventAggregator.GetEvent<AuthBarEvents.LogOutCompleted>().Subscribe(onLogOutCompleted);
      }

      private void onAthorizationCompleted(bool result) {
         if (result == true) {
            _eventAggregator.GetEvent<WallSelectorEvents.FillWallRequest>().Publish();
         }
         else {
            _regionManager.RequestNavigate(RegionNames.MainRegion, "StartPage");
         }
      }

      private void onLogOutCompleted() {
         _regionManager.RequestNavigate(RegionNames.MainRegion, "StartPage");
      }

      private void onSettingsRequested() {
         var settings = new SettingsView();
         settings.ShowDialog();
      }

      private void onBackRequested() {
         _regionManager.RequestNavigate(RegionNames.MainRegion, "AvailableWalls");
         _eventAggregator.GetEvent<ShellEvents.WallSelectedEvent>().Publish(false);
      }

      private async void onWallItemClicked(WallItem wallItem) {
         try {
            //IsBusy = true;
            //await Wall.PullWithScheduleHightlightAsync(wallItem.WallHolder, CurrentPostTypeFilter.GetFilter(),
            //      CurrentSchedule);
            _regionManager.RequestNavigate(RegionNames.MainRegion, "Content");
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

         var mainVmData = await SaveLoaderHelper.TryLoadAsync<MainVMSaveInfo>("MainVM");
         if (mainVmData.Successful) {
            //TestingGroup = data.TestingGroup;
         }

         var loadedSettings = await SaveLoaderHelper.TryLoadAsync<Settings>("Settings");
         var currentSettings = App.Container.GetInstance<Settings>();

         if (loadedSettings.Successful) {
            currentSettings.ApplySettings(loadedSettings.Result);
         }
         else {
            //defaults
            currentSettings.ApplySettings(new Settings());
         }

         //SetUpAvatar(DEFAULT_AVATAR);

         //await authorizeIfAlreadyLoggined();
         _eventAggregator.GetEvent<AuthBarEvents.AuthorizeIfAlreadyLoggined>().Publish();

         //IsBusy = false;

         //_eventAggregator.GetEvent<FillWallListEvent>().Publish();
      }

      public void OnClosing() {
         var some = App.Container.GetInstance<Settings>();
         SaveLoaderHelper.Save("MainVM", new MainVMSaveInfo());
         SaveLoaderHelper.Save("Settings", some);
      }
   }
}