using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class Voucher
{
    public string VoucherId { get; set; } = null!;

    public string VoucherName { get; set; } = null!;

    public double VoucherValue { get; set; }

    public int? VoucherPoint { get; set; }

    public int VoucherQuantity { get; set; }

    public DateTime? DateExpire { get; set; }

    public string? EmployeeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<VoucherDetail> VoucherDetails { get; set; } = new List<VoucherDetail>();
}
