using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FigureManagementSystem.Helpers
{
    public class MessageRoleToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var role = value as string;
            if (role == null)
                return HorizontalAlignment.Left;

            // You can adjust to fit your role names ("user", "assistant", etc.)
            return role.Equals("user", StringComparison.OrdinalIgnoreCase)
                ? HorizontalAlignment.Right    // User messages align right
                : HorizontalAlignment.Left;    // Assistant messages align left
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}