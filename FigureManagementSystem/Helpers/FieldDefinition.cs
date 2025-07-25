using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureManagementSystem.Helpers
{
    public class FieldDefinition
    {
        public string Label { get; set; }
        public string PropertyName { get; set; }
        public Type Type { get; set; }
        public bool IsReadOnly { get; set; } = false;
    }
}
