using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblOrder
{
    public int OrderId { get; set; }

    public DateOnly OrderDate { get; set; }

    public decimal Total { get; set; }

    public string UserId { get; set; } = null!;

    public string? DiscountId { get; set; }

    public int? PaymentId { get; set; }

    public int? ShippingId { get; set; }

    public bool? IsActive { get; set; }

    public virtual TblDiscount? Discount { get; set; }

    public virtual TblPayment? Payment { get; set; }

    public virtual TblShipping? Shipping { get; set; }

    public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; } = new List<TblOrderDetail>();

    public virtual ICollection<TblPayment> TblPayments { get; set; } = new List<TblPayment>();

    public virtual ICollection<TblShipping> TblShippings { get; set; } = new List<TblShipping>();

    public virtual TblUser User { get; set; } = null!;
}
