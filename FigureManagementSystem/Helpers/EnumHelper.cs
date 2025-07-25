using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace FigureManagementSystem.Helpers
{

    public static class EnumHelper
    {
        public static string GetEnumMemberValue<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            var type = typeof(TEnum);
            var name = enumValue.ToString();
            var field = type.GetField(name)!;

            var attr = field.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                            .Cast<EnumMemberAttribute>()
                            .FirstOrDefault();

            return attr?.Value ?? name;
        }

        public static TEnum GetEnumFromValue<TEnum>(string value) where TEnum : Enum
        {
            foreach (var field in typeof(TEnum).GetFields())
            {
                var attr = field.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                                .Cast<EnumMemberAttribute>()
                                .FirstOrDefault();

                if ((attr?.Value ?? field.Name) == value)
                    return (TEnum)field.GetValue(null)!;
            }

            throw new ArgumentException($"Requested value '{value}' was not found in enum {typeof(TEnum).Name}");
        }
    }

}
