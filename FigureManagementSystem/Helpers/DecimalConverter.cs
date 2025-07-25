using System;
using System.Globalization;
using System.Windows.Data;

public class DecimalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal dec)
            return dec.ToString(culture);
        return string.Empty;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string strValue = value as string;
        if (decimal.TryParse(strValue, NumberStyles.Any, culture, out decimal result))
            return result;

        return Binding.DoNothing;
    }
}
