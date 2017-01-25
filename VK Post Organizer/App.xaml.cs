using System;
using System.Windows;
using System.Windows.Threading;
using Data_Persistence_Provider;
using SimpleInjector;
using vk.Models;
using vk.Models.Configuration;
using vk.Models.UrlHelper;
using vk.Models.VkApi;
using vk.Views;

namespace vk {
   public partial class App : Application {
      public static Container Container { get; set; }

      private void CompositionRoot(object sender, StartupEventArgs e) {
         if (SingleInstance.IsOnlyInstance() == false) {
            //SingleInstance.ShowFirstInstance();
            Current.Shutdown();
            return;
         }

         Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
         Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

         Container = Bootstrap();

         var window = Container.GetInstance<MainView>();
         window.Show();
      }

      public static Container Bootstrap() {
         var container = new Container();

         container.Register<AccessToken>(Lifestyle.Singleton);
         container.Register<Settings>(Lifestyle.Singleton);

         container.Register<ISerializer, SaveLoadJsonSerializer>();
         container.Register<IDataProvider, AppDataFolderProvider>();
         //container.Register<SaveLoadController>();

         container.Register<IWebClient, WebClientWithProxy>();
         container.Register<IWallHolder, EmptyWallHolder>();

         container.Register<PhotoUrlObtainer>(Lifestyle.Singleton);
         container.Register<DocumentPreviewUrlObtainer>(Lifestyle.Singleton);
         //container.Register<ImageExtensionChecker>();
         //container.Register<UploadWindow>();
         container.Verify();

         return container;
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
