using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureManagementSystem.Helpers
{
    public class DisplayConverter : System.Windows.Data.IValueConverter
    {
        private readonly Func<object, string> _display;

        public DisplayConverter(Func<object, string> display) => _display = display;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) =>
            _display(value);

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
