using System;
using System.Windows;
using System.Windows.Threading;
using Data_Persistence_Provider;
using Microsoft.Practices.Unity;
using vk.Models;
using vk.Models.Files;
using vk.Models.VkApi;
using vk.Views;

namespace vk {
   public partial class App : Application {
      private static IUnityContainer _container;

      public static IUnityContainer Container
      {
         get { return _container; }
         private set { _container = value; }
      }

      private void CompositionRoot(object sender, StartupEventArgs e) {
         Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
         Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

         Container = new UnityContainer();

         Container.RegisterType<Settings>(new ContainerControlledLifetimeManager());

         Container.RegisterType<ISerializer, SaveLoadJsonSerializer>();
         Container.RegisterType<IDataProvider, AppDataFolderProvider>();
         Container.RegisterType<SaveLoadController>(new ContainerControlledLifetimeManager());

         Container.RegisterType<IWebClient, WebClientWithProxy>();
         Container.RegisterType<IWallHolder, EmptyWallHolder>();
         Container.RegisterType<EmptyWallHolder>(new ContainerControlledLifetimeManager());

         Container.RegisterType<ImageExtensionChecker>(new ContainerControlledLifetimeManager());

         Container.RegisterType<StatsTrackVisitor>(new ContainerControlledLifetimeManager());

         var window = new MainView();
         window.Show();
      }

      private void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
         MessageBox.Show($"{e.Exception.Message}\n\n See dump at Windows Application Event Log.\nStack Trace:{e.Exception.StackTrace}", 
            e.Exception.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);

#if !DEBUG
         e.Handled = true;
#endif

         Environment.FailFast("Exception occured", e.Exception);
      }
   }
}
