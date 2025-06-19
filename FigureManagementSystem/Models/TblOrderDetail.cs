using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblOrderDetail
{
    public int DetailId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public bool? IsActive { get; set; }

    public virtual TblOrder Order { get; set; } = null!;

    public virtual TblProduct Product { get; set; } = null!;
}
