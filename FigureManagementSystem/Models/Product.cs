using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImagePath { get; set; }
    public string AbsoluteImagePath
    {
        get
        {
            if (string.IsNullOrEmpty(ImagePath))
                return null;

            if (System.IO.Path.IsPathRooted(ImagePath))
                return ImagePath;

            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagePath);
        }
    }


    public int BrandId { get; set; }

    public int CharacterId { get; set; }

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual Character Character { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
