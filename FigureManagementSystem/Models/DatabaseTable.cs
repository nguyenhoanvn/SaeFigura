using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureManagementSystem.Models
{
    public class DatabaseTable
    {
        public string Name { get; set; }
        public string Display => Name;
    }
}
