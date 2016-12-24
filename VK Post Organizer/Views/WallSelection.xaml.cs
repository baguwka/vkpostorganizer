using System.Windows;
using System.Windows.Controls;
using vk.Models;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for WallSelection.xaml
   /// </summary>
   public partial class WallSelection : UserControl {
      public static readonly DependencyProperty WallControllerProperty =
         DependencyProperty.Register
            (
               "WallController",
               typeof (WallList),
               typeof (WallSelection)
            );

      public WallList WallController
      {
         get { return (WallList)GetValue(WallControllerProperty); }
         set { SetValue(WallControllerProperty, value); }
      }

      public WallSelection() {
         InitializeComponent();
      }
   }
}