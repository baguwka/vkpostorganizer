using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using vk.Models;

namespace vk.Converter {
   class MissingPostIsShowingConverter : IValueConverter {
      public Visibility visibility;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (!(value is PostType)) return Visibility.Visible;

         var postMark = (PostType)value;

         switch (postMark) {
            case PostType.Missing:
               return visibility = Visibility.Visible;
            default:
               return visibility = Visibility.Collapsed;
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         return visibility;
      }
   }

   class MissingPostIsHiddenConverter : IValueConverter {
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