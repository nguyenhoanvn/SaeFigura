using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblBrand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public decimal? AverageRate { get; set; }

    public DateOnly? ActiveDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TblProduct> TblProducts { get; set; } = new List<TblProduct>();
}
