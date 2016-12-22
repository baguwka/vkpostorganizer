using System;
using System.Globalization;
using System.Windows.Data;
using vk.ViewModels;

namespace vk.Converter {
   class PostMarkToColorConverter : IValueConverter {
      public string color;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (!(value is PostMark)) return "#ffffff";

         var postMark = (PostMark)value;

         switch (postMark) {
            case PostMark.Bad:
               return color = "#d6b6b6";
            case PostMark.Good:
               return color = "#B9D6B6";
            case PostMark.Neutral:
               return color = "#ffffff";
         }

         return color = "#ffffff";
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         return color;
      }
   }
}