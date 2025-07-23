using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureManagementSystem.Helpers
{
    public class FilterItem : INotifyPropertyChanged
    {
        public string PropertyName { get; set; } = "";
        public string Label { get; set; } = "";
        public string DisplayMemberPath { get; set; } = "Name";
        private object? _selectedFilterValue;
        public object? SelectedFilterValue
        {
            get => _selectedFilterValue;
            set
            {
                if (_selectedFilterValue != value)
                {
                    _selectedFilterValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFilterValue)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
