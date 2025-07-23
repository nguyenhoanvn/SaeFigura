using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureManagementSystem.Helpers
{
    public class LinkedEntityDefinition
    {
        public string Label { get; set; } = "";
        public string PropertyName { get; set; } = ""; 
        public string DisplayMemberPath { get; set; } = "Name";
        public Type LinkedEntityType { get; set; }     
        public Func<IEnumerable<object>> ItemsSourceProvider { get; set; } = () => Enumerable.Empty<object>();
        public Func<object, string> DisplayMemberSelector { get; set; } = obj => obj.ToString()!;
    }
}
