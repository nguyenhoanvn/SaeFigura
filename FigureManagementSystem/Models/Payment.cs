/*using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FigureManagementSystem.Models;

public enum PaymentMethod
{
    [EnumMember(Value = "Credit Card")]
    CreditCard,

    [EnumMember(Value = "Cash")]
    Cash,

    [EnumMember(Value = "Bank Transfer")]
    BankTransfer
}

public enum PaymentStatus
{
    Completed,
    Pending
}

public partial class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public PaymentMethod Method { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; }

    public DateOnly PaymentDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Order Order { get; set; } = null!;
}
*/