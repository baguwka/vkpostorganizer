using System;
using System.Globalization;
using System.Windows.Data;
using vk.Models;

namespace vk.Converter {
   class PostTypeToBoolConverter : IValueConverter {
      public bool enabled;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (!(value is PostType)) return true;

         var postMark = (PostType)value;

         switch (postMark) {
            case PostType.Missing:
               return enabled = false;
            default:
               return enabled = true;
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         return enabled;
      }
   }
}