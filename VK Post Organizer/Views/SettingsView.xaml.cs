using System.Windows;
using System.Windows.Input;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for SettingsWindow.xaml
   /// </summary>
   public partial class SettingsView : Window {
      public SettingsView() {
         InitializeComponent();
      }

      private void onCloseClick(object sender, RoutedEventArgs e) {
         Close();
      }

      private void SettingsView_OnKeyDown(object sender, KeyEventArgs e) {
         if (e.Key == Key.Escape) {
            this.Close();
         }
      }
   }
}
