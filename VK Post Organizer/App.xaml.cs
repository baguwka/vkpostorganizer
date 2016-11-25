using System.ComponentModel;
using System.Windows;
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

         Container.RegisterType<IWebClient, DefaultWebClient>();

         var window = new MainView();
         window.Show();
      }
   }
}
