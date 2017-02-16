using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Unity;
using vk.Views;

namespace vk {
   public class Bootstrapper : UnityBootstrapper {
      protected override DependencyObject CreateShell() {
         return Container.Resolve<MainView>();
      }

      protected override void InitializeShell() {
         Application.Current.MainWindow.Show();
      }

      protected override void ConfigureContainer() {
         base.ConfigureContainer();
         Container.RegisterTypeForNavigation<AvailableWallsView>("AvailableWalls");
         Container.RegisterTypeForNavigation<MainBottomView>("MainBottomButtons");
         Container.RegisterTypeForNavigation<AuthBarView>("AuthBar");
         Container.RegisterTypeForNavigation<StartPageView>("StartPage");
      }
   }
}