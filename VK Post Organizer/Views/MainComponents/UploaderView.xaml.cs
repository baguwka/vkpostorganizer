using System.Windows.Controls;
using System.Windows.Input;
using vk.ViewModels;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for UploaderView.xaml
   /// </summary>
   public partial class UploaderView : UserControl {
      private readonly UploaderViewModel _viewModel;

      public UploaderView() {
         InitializeComponent();
         _viewModel = (UploaderViewModel)DataContext;
      }

      private void onMouseWheel(object sender, MouseWheelEventArgs e) {
         if (e.Delta > 0) {
            _viewModel.moveToNextMissing();
         }
         else {
            _viewModel.moveToPreviousMissing();
         }
      }
   }
}
