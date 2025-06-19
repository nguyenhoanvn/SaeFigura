using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblShipping
{
    public int ShippingId { get; set; }

    public int OrderId { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public string ShippingMethod { get; set; } = null!;

    public decimal ShippingCost { get; set; }

    public string ShippingStatus { get; set; } = null!;

    public DateOnly ShippedDate { get; set; }

    public DateOnly EstimatedDate { get; set; }

    public DateOnly? ActualDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual TblOrder Order { get; set; } = null!;

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();
}
