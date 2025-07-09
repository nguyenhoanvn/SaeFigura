using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FigureManagementSystem.Helpers
{
    public class LinkedEntitySourceConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is Dictionary<string, IEnumerable<object>> sources && values[1] is string propertyName)
            {
                if (sources.TryGetValue(propertyName, out var result))
                    return result;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
