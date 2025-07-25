using System;
using System.Globalization;
using System.Windows.Data;

public class IntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return string.Empty;
        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string input = value as string;

        if (string.IsNullOrWhiteSpace(input))
        {
            if (targetType == typeof(int?)) return null;
            return Binding.DoNothing;
        }

        if (int.TryParse(input, NumberStyles.Integer, culture, out int result))
        {
            return result;
        }

        return Binding.DoNothing;
    }
}
