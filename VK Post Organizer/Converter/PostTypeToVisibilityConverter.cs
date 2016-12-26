using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using vk.Models;

namespace vk.Converter {
   class PostTypeToVisibilityConverter : IValueConverter {
      public Visibility visibility;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (!(value is PostType)) return Visibility.Visible;

         var postMark = (PostType)value;

         switch (postMark) {
            case PostType.Missing:
               return visibility = Visibility.Collapsed;
            default:
               return visibility = Visibility.Visible;
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         return visibility;
      }
   }
}