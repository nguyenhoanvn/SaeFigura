using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblMaterial
{
    public int MaterialId { get; set; }

    public string MaterialName { get; set; } = null!;

    public decimal Density { get; set; }

    public string? MaterialDescription { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TblProduct> TblProducts { get; set; } = new List<TblProduct>();
}
