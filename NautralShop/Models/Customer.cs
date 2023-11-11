using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class Customer
{
    public string CustomerId { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string? CustomerAddress { get; set; }

    public string? CustomerEmail { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerUsername { get; set; }

    public string? CustomerPassword { get; set; }

    public int? CustomerPoint { get; set; }

    public bool CustomerStatus { get; set; }

    public int? FunctionId { get; set; }

    public virtual Function? Function { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<VoucherDetail> VoucherDetails { get; set; } = new List<VoucherDetail>();
}
