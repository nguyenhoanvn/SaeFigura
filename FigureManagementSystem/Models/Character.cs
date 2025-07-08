using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class Character
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int SeriesId { get; set; }

    public string MainColor { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual Series Series { get; set; } = null!;
}
