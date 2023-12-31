﻿using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public double? ProductPrice { get; set; }

    public string? ProductImage { get; set; }

    public double? ProductValuePromotion { get; set; }

    public string? ProductIngredient { get; set; }

    public string? ProductUseful { get; set; }

    public string? ProductUserManual { get; set; }

    public string? ProductDescription { get; set; }

    public string? ProductDetailDescription { get; set; }

    public bool ProductStatus { get; set; }
    public int? CategoryId { get; set; }
    //public string? CategoryName { get; set; } = string.Empty!;
    public int ProductQuantity { get; set; } = 0!;
    public string? ProductImage2 { get; set; } = string.Empty!;
    public string? ProductImage3 { get; set; } = string.Empty!;


    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
