using System.Windows;
using JetBrains.Annotations;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for ScheduleItemEdit.xaml
   /// </summary>
   public partial class ScheduleItemEdit : Window {
      public ScheduleItemEdit([NotNull] string name) {
         InitializeComponent();

         NameBox.Text = name;
      }
   }
}
