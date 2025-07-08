using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class Brand
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? AverageRating { get; set; }

    public DateOnly? ActiveSince { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
