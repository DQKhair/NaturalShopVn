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

    //public string OrderCustomerName { get; set; } = string.Empty;
    //public string OrderCustomerPhone {  get; set; } = string.Empty;
    //public string OrderCustomerAddress { get; set; } = string.Empty;
    //public string? ProductName { get; set; } = string.Empty;
    //public string? StatusOrderName {  get; set; } = string.Empty;
    //public int StatusOrderId { get; set; }
    //public string? CustomerId { get; set; }
    //public string? employeeId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
