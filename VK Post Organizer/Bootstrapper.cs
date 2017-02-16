using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Unity;
using vk.Infrastructure;
using vk.Views;

namespace vk {
   public class Bootstrapper : UnityBootstrapper {
      protected override DependencyObject CreateShell() {
         return Container.Resolve<Shell>();
      }

      protected override void InitializeShell() {
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
      }
   }
}