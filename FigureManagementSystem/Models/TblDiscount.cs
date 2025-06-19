using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblDiscount
{
    public string DiscountId { get; set; } = null!;

    public string DiscountName { get; set; } = null!;

    public DateOnly ActivateDate { get; set; }

    public DateOnly ExpireDate { get; set; }

    public string? DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();
}
