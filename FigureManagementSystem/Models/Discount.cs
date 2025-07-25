using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public enum DiscountType
{
    Fixed,
    Percent
}

public partial class Discount
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly ActivateDate { get; set; }

    public DateOnly ExpireDate { get; set; }

    public DiscountType Type { get; set; }

    public decimal Value { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
