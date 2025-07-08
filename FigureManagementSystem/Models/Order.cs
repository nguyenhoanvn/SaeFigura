using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly OrderDate { get; set; }

    public decimal Total { get; set; }

    public string UserId { get; set; } = null!;

    public string? DiscountId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
