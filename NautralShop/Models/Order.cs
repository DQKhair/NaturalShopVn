using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public int OrderTotalQuantity { get; set; }

    public int OrderTotalPrice { get; set; }

    public string OrderCustomerName { get; set; } = null!;

    public string OrderCustomerPhone { get; set; } = null!;

    public string OrderCustomerAddress { get; set; } = null!;

    public string? CustomerId { get; set; }

    public string? EmployeeId { get; set; }

    public int? PaymentMethodId { get; set; }

    public string PaymentMethodName { get; set; } = string.Empty;

    public int? StatusOrderId { get; set; }

    public string StatusOrderName { get; set; } = string.Empty;

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual StatusOrder? StatusOrder { get; set; }
}
