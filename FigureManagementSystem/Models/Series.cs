using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class Series
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
}
