using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string ProductName => Product?.Name ?? string.Empty;

    public bool? IsActive { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
