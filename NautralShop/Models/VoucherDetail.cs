using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class VoucherDetail
{
    public string VoucherDetailId { get; set; } = null!;

    public int? VoucherDetailQuantity { get; set; }

    public string? VoucherId { get; set; }

    public string? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
