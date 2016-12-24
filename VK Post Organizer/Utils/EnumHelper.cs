using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace vk.Utils {
   public class ValueDescription {
      public Enum Value { get; set; }
      public string Description { get; set; }
   }

   public static class EnumHelper {
      /// <summary>
      /// Gets the description of a specific enum value.
      /// </summary>
      public static string Description(this Enum eValue) {
         var nAttributes = eValue.GetType().GetField(eValue.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

         if (!nAttributes.Any()) {
            var oTI = CultureInfo.CurrentCulture.TextInfo;
            return oTI.ToTitleCase(oTI.ToLower(eValue.ToString().Replace("_", " ")));
         }

         var descriptionAttribute = nAttributes.First() as DescriptionAttribute;
         return descriptionAttribute != null ? descriptionAttribute.Description : eValue.ToString();
      }

      /// <summary>
      /// Returns an enumerable collection of all values and descriptions for an enum type.
      /// </summary>
      public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions<TEnum>() where TEnum : struct, IConvertible, IComparable, IFormattable {
         if (!typeof(TEnum).IsEnum)
            throw new ArgumentException("TEnum must be an Enumeration type");

         return from e in Enum.GetValues(typeof(TEnum)).Cast<Enum>()
                select new ValueDescription() { Value = e, Description = e.Description() };
      }
   }
}
