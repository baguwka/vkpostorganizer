using System.ComponentModel;
using System.Windows;
using Microsoft.Practices.Unity;
using vk.Models;
using vk.ViewModels;
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

         Container.RegisterType<IWebClient, DefaultWebClient>();
         Container.RegisterInstance(new EmptyWallHolder());

         var window = new MainView();
         window.Show();
      }
   }
}
