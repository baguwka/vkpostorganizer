﻿using System.Windows;
using Data_Persistence_Provider;
using Microsoft.Practices.Unity;
using vk.Models;
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
         Container = new UnityContainer();

         Container.RegisterType<ISerializer, SaveLoadJsonSerializer>();
         Container.RegisterType<IDataProvider, AppDataFolderProvider>();
         Container.RegisterType<SaveLoadController>(new ContainerControlledLifetimeManager());

         Container.RegisterType<IWebClient, DefaultWebClient>();
         Container.RegisterType<IWallHolder, EmptyWallHolder>();
         Container.RegisterType<EmptyWallHolder>(new ContainerControlledLifetimeManager());

         var window = new MainView();
         window.Show();
      }
   }
}
