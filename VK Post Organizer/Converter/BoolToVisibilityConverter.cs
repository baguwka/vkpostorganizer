using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace vk.Converter {
   class BoolToVisibilityHiddenConverter : IValueConverter {
      public Visibility visibility;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (!(value is bool)) return Visibility.Visible;

         return ((bool)value) ? visibility = Visibility.Visible : visibility = Visibility.Hidden;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         return visibility;
      }
   }
   class BoolToVisibilityCollapsedConverter : IValueConverter {
      public Visibility visibility;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (!(value is bool)) return Visibility.Visible;

         return ((bool)value) ? visibility = Visibility.Visible : visibility = Visibility.Collapsed;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         return visibility;
      }
   }

   class BoolToVisibilityNegativeCollapsedConverter : IValueConverter {
      public Visibility visibility;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (!(value is bool)) return Visibility.Visible;

         return (!(bool)value) ? visibility = Visibility.Visible : visibility = Visibility.Collapsed;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         return visibility;
      }
   }
}