using System.Windows;
using System.Windows.Controls;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for ScheduleWindow.xaml
   /// </summary>
   public partial class ScheduleWindow : Window {
      public ScheduleWindow() {
         InitializeComponent();
      }

      //todo: use blend triggers and commands instead of this
      private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      }

      private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
         ListBox.Items.Refresh();
      }
   }
}
