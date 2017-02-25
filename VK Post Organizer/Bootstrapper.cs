using System.Net.Http;
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
using vk.ViewModels;
using vk.Views;

namespace vk {
   public class Bootstrapper : UnityBootstrapper {
      protected override DependencyObject CreateShell() {
         return Container.Resolve<Shell>();
      }

      protected override void InitializeShell() {
         var regionManager = Container.Resolve<IRegionManager>();

         var settings = Container.Resolve<Settings>();
         var saveLoader = Container.Resolve<VkPostponeSaveLoader>();
         var loadedSettings = saveLoader.TryLoad<Settings>("Settings");
         settings.ApplySettings(loadedSettings.Successful ? loadedSettings.Result : new Settings());

         var historyController = Container.Resolve<HistoryController>();
         historyController.Observe();

         regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(StartPageView));
         regionManager.RegisterViewWithRegion(RegionNames.BottomRegion, typeof(MainBottomView));
         regionManager.RegisterViewWithRegion(RegionNames.AuthRegion, typeof(AuthBarView));

         regionManager.RegisterViewWithRegion(RegionNames.ContentLeftBlockRegion, typeof(WallContentLeftBlockView));
         regionManager.RegisterViewWithRegion(RegionNames.ContentMainRegion, typeof(StartPageView));
         regionManager.RegisterViewWithRegion(RegionNames.ContentOverlayRegion, typeof(UploaderView));

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

         Container.RegisterType<Settings>(Lifetime.Singleton);
         Container.RegisterType<ProxySettings>(new InjectionFactory(container => container.Resolve<Settings>().Proxy));
         Container.RegisterType<UploadSettings>(new InjectionFactory(container => container.Resolve<Settings>().Upload));

         Container.RegisterType<HttpClientHandlerFactory>(Lifetime.Singleton);
         Container.RegisterType<HttpMessageHandler>(new InjectionFactory(container =>
            container.Resolve<HttpClientHandlerFactory>().BuildHttpClientHandlerWithProxyIfEnabled()));
         Container.RegisterType<HttpClientHandler>(new InjectionFactory(container =>
            container.Resolve<HttpClientHandlerFactory>().BuildHttpClientHandlerWithProxyIfEnabled()));

         Container.RegisterType<AccessToken>(Lifetime.Singleton);

         Container.RegisterType<VkApi>(Lifetime.Singleton);
         Container.RegisterType<VkApiProvider>(Lifetime.Singleton);

         Container.RegisterType<GroupsGet>(new InjectionFactory(container => container.Resolve<VkApiProvider>().GroupsGet));
         Container.RegisterType<GroupsGetById>(new InjectionFactory(container => container.Resolve<VkApiProvider>().GroupsGetById));
         Container.RegisterType<PhotosGetById>(new InjectionFactory(container => container.Resolve<VkApiProvider>().PhotosGetById));
         Container.RegisterType<PhotosGetWallUploadSever>(new InjectionFactory(container => container.Resolve<VkApiProvider>().PhotosGetWallUploadSever));
         Container.RegisterType<PhotosSaveWallPhoto>(new InjectionFactory(container => container.Resolve<VkApiProvider>().PhotosSaveWallPhoto));
         Container.RegisterType<StatsTrackVisitor>(new InjectionFactory(container => container.Resolve<VkApiProvider>().StatsTrackVisitor));
         Container.RegisterType<UsersGet>(new InjectionFactory(container => container.Resolve<VkApiProvider>().UsersGet));
         Container.RegisterType<WallGet>(new InjectionFactory(container => container.Resolve<VkApiProvider>().WallGet));
         Container.RegisterType<WallPost>(new InjectionFactory(container => container.Resolve<VkApiProvider>().WallPost));

         Container.RegisterType<ISerializer, SaveLoadJsonSerializer>();
         Container.RegisterType<IDataProvider, AppDataFolderProvider>();

         Container.RegisterType<IUsersGet, UsersGet>();
         Container.RegisterType<IGroupsGet, GroupsGet>();
         Container.RegisterType<IPhotosGetWallUploadSever, PhotosGetWallUploadSever>();
         Container.RegisterType<IPhotosSaveWallPhoto, PhotosSaveWallPhoto>();

         Container.RegisterType<VkPostponeSaveLoader>(Lifetime.Singleton);

         Container.RegisterType<IWallHolder, EmptyWallHolder>();
         Container.RegisterType<SharedWallContext>(Lifetime.Singleton);
         Container.RegisterType<WallContainerController>(Lifetime.Singleton);

         Container.RegisterType<HistoryController>(Lifetime.Singleton);
         Container.RegisterType<IHistoryPublisher, JsonServerHistoryPublisher>(Lifetime.Singleton);

         Container.RegisterType<PhotoUrlObtainer>(Lifetime.Singleton);
         Container.RegisterType<DocumentPreviewUrlObtainer>(Lifetime.Singleton);
      }
   }

   public static class Lifetime {
      public static LifetimeManager Singleton => new ContainerControlledLifetimeManager();
   }
}