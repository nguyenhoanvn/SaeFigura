using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureManagementSystem.Helpers
{
    public class ForeignKeyMapping
    {
        public Type EntityType { get; set; }
        public string DisplayProperty { get; set; } = "";
    }

}
