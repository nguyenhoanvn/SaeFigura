using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblUser
{
    public string UserId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public bool? IsActive { get; set; }

    public virtual TblRole Role { get; set; } = null!;

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();

    public virtual ICollection<TblPayment> TblPayments { get; set; } = new List<TblPayment>();
}
