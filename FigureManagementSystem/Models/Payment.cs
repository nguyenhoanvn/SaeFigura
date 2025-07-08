using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string Method { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? Currency { get; set; }

    public string? Status { get; set; }

    public DateOnly PaymentDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Order Order { get; set; } = null!;
}
