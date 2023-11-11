using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class OrderDetail
{
    public string OrderDetailId { get; set; } = null!;

    public int OrderDetailQuantity { get; set; }

    public double OrderDetailPrice { get; set; }

    public string? OrderId { get; set; }

    public string? ProductId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
