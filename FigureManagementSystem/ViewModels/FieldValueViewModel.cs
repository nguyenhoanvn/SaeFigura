using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureManagementSystem.ViewModels
{
    public class FieldValueViewModel : INotifyPropertyChanged
    {
        public string Label { get; }
        public string PropertyName { get; }
        public Type FieldType { get; }
        private object? _value;
        public object? Value;
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public FieldValueViewModel(string label, string propertyName, Type fieldType, object? initialValue = null)
        {
            Label = label;
            PropertyName = propertyName;
            FieldType = fieldType;
            _value = initialValue;
        }
    }
}
