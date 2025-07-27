using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FigureManagementSystem.Helpers
{
    public class BoolToSelectBrushConverter : IValueConverter
    {
        public Brush SelectedBrush { get; set; } = new SolidColorBrush(Color.FromRgb(41, 128, 234)); // #2980EA
        public Brush UnselectedBrush { get; set; } = new SolidColorBrush(Color.FromRgb(221, 221, 221)); // #DDD

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
                return SelectedBrush;
            return UnselectedBrush;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
