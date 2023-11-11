using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class Function
{
    public int FunctionId { get; set; }

    public string FunctionName { get; set; } = null!;

    public string? FunctionIcon { get; set; }

    public string? RouteAction { get; set; }

    public string? RouteController { get; set; }

    public string? RouteArea { get; set; }

    public int? AccountTypeId { get; set; }

    public virtual AccountType? AccountType { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
