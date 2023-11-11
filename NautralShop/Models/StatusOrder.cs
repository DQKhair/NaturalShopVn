using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class StatusOrder
{
    public int StatusOrderId { get; set; }

    public string StatusOrderName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
