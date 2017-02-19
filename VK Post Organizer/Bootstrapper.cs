using System.Windows;
using Data_Persistence_Provider;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;
using vk.Infrastructure;
using vk.Models;
using vk.Models.Logger;
using vk.Models.UrlHelper;
using vk.Models.VkApi;
using vk.Views;

namespace vk {
   public class Bootstrapper : UnityBootstrapper {
      protected override DependencyObject CreateShell() {
         return Container.Resolve<Shell>();
      }

      protected override void InitializeShell() {
         var regionManager = Container.Resolve<IRegionManager>();

         regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(StartPageView));
         regionManager.RegisterViewWithRegion(RegionNames.BottomRegion, typeof(MainBottomView));
         regionManager.RegisterViewWithRegion(RegionNames.AuthRegion, typeof(AuthBarView));

         regionManager.RegisterViewWithRegion(RegionNames.ContentLeftBlockRegion, typeof(WallContentLeftBlockView));
         regionManager.RegisterViewWithRegion(RegionNames.ContentMainRegion, typeof(StartPageView));

         Application.Current.MainWindow.Show();
      }

      protected override void ConfigureContainer() {
         base.ConfigureContainer();
         Container.RegisterTypeForNavigation<AvailableWallsView>(ViewNames.AvailableWalls);
         Container.RegisterTypeForNavigation<MainBottomView>(ViewNames.MainBottomButtons);
         Container.RegisterTypeForNavigation<AuthBarView>(ViewNames.AuthBar);
         Container.RegisterTypeForNavigation<StartPageView>(ViewNames.StartPage);

         Container.RegisterTypeForNavigation<ContentView>(ViewNames.Content);
         Container.RegisterTypeForNavigation<WallPostponeContentView>(ViewNames.WallPostponeContent);
         Container.RegisterTypeForNavigation<WallActualContentView>(ViewNames.WallActualContent);
         Container.RegisterTypeForNavigation<HistoryContentView>(ViewNames.HistoryContent);

         Container.RegisterType<AccessToken>(Lifetime.Singleton);
         Container.RegisterType<Settings>(Lifetime.Singleton);

         Container.RegisterType<ISerializer, SaveLoadJsonSerializer>();
         Container.RegisterType<IDataProvider, AppDataFolderProvider>();

         Container.RegisterType<IUsersGet, UsersGet>();
         Container.RegisterType<IGroupsGet, GroupsGet>();
         Container.RegisterType<IPhotosGetWallUploadSever, PhotosGetWallUploadSever>();
         Container.RegisterType<IPhotosSaveWallPhoto, PhotosSaveWallPhoto>();

         Container.RegisterType<VkUploaderFactory>(Lifetime.Singleton);
         Container.RegisterType<VkUploader>(new InjectionFactory(container => container.Resolve<VkUploaderFactory>().BuildNewVkUploader()));

         Container.RegisterType<VkPostponeSaveLoader>(Lifetime.Singleton);
         //container.Register<SaveLoadController>();

         Container.RegisterType<IWebClient, WebClientWithProxy>();
         Container.RegisterType<IWallHolder, EmptyWallHolder>();

         Container.RegisterType<IPublishLogger, JsonServerPublishLogger>();

         Container.RegisterType<PhotoUrlObtainer>(Lifetime.Singleton);
         Container.RegisterType<DocumentPreviewUrlObtainer>(Lifetime.Singleton);
      }
   }

   public static class Lifetime {
      public static LifetimeManager Singleton => new ContainerControlledLifetimeManager();
   }
}