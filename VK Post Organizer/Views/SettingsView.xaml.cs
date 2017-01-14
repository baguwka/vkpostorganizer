﻿using System.Text.RegularExpressions;
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
   }
}