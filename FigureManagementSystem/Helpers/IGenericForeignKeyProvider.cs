using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureManagementSystem.Helpers
{
    public interface IGenericForeignKeyProvider
    {
        Dictionary<string, ForeignKeyMapping> ForeignKeyMappings { get; }
    }
}
