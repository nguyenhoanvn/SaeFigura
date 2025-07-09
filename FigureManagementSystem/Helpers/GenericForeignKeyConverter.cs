using FigureManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FigureManagementSystem.Helpers
{
    public class GenericForeignKeyConverter : IValueConverter
    {
        private readonly Type _entityType;
        private readonly string _displayProperty;

        public GenericForeignKeyConverter(Type entityType, string displayProperty)
        {
            _entityType = entityType;
            _displayProperty = displayProperty;
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            using var context = new FigureManagementSystemContext();
            var entity = context.Find(_entityType, value);
            return entity?.GetType().GetProperty(_displayProperty)?.GetValue(entity);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

}
