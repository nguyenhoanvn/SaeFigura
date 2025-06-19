using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblProduct
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Image { get; set; }

    public int BrandId { get; set; }

    public int CharacterId { get; set; }

    public int MaterialId { get; set; }

    public decimal Price { get; set; }

    public decimal Weight { get; set; }

    public decimal Height { get; set; }

    public decimal Width { get; set; }

    public int Quantity { get; set; }

    public int CategoryId { get; set; }

    public DateOnly? ImportDate { get; set; }

    public DateOnly? UsingDate { get; set; }

    public bool? IsActive { get; set; }

    public string? ProductSlug { get; set; }

    public virtual TblBrand Brand { get; set; } = null!;

    public virtual TblCategory Category { get; set; } = null!;

    public virtual TblCharacter Character { get; set; } = null!;

    public virtual TblMaterial Material { get; set; } = null!;

    public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; } = new List<TblOrderDetail>();
}
