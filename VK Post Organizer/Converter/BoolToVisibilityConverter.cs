using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace vk.Converter {
   [ValueConversion(typeof(bool), typeof(Visibility))]
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

   [ValueConversion(typeof(bool), typeof(Visibility))]
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

   [ValueConversion(typeof(bool), typeof(Visibility))]
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