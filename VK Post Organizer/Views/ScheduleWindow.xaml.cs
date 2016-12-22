﻿using System.Windows;
using System.Windows.Controls;
using vk.ViewModels;

namespace vk.Views {
   /// <summary>
   /// Interaction logic for ScheduleWindow.xaml
   /// </summary>
   public partial class ScheduleWindow : Window {
      private IVM getViewModel => (IVM)DataContext;

      public ScheduleWindow() {
         InitializeComponent();
      }

      //todo: use blend triggers and commands instead of this
      private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
         var vm = (ScheduleVM)getViewModel;
         vm.OnSelectionChange();
      }

      private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
         ListBox.Items.Refresh();
      }
   }
}