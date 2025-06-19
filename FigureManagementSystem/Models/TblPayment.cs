using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblPayment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public string? UserId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal PaymentAmount { get; set; }

    public string Currency { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    public DateOnly PaymentDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual TblOrder Order { get; set; } = null!;

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();

    public virtual TblUser? User { get; set; }
}
