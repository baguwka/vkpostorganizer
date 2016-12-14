using System.Windows;
using vk.ViewModels;

namespace vk.Views {
   public partial class MainView : Window {
      public IVM GetViewModel => (IVM)DataContext;

      public MainView() {
         InitializeComponent();

      }

      private void onLoaded(object sender, RoutedEventArgs e) {
         GetViewModel.OnLoad();
      }
   }
}
