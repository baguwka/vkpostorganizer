using System.Windows.Controls;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for WallSelection.xaml
   /// </summary>
   public partial class AvailableWallsView : UserControl {
      //public static readonly DependencyProperty WallControllerProperty =
      //   DependencyProperty.Register
      //      (
      //         "WallController",
      //         typeof (WallList),
      //         typeof (AvailableWallsView)
      //      );

      //public WallList WallController
      //{
      //   get { return (WallList)GetValue(WallControllerProperty); }
      //   set { SetValue(WallControllerProperty, value); }
      //}

      public AvailableWallsView() {
         InitializeComponent();
      }
   }
}