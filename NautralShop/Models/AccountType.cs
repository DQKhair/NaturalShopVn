using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class AccountType
{
    public int AccountTypeId { get; set; }

    public string AccountTypeName { get; set; } = null!;

    public virtual ICollection<Function> Functions { get; set; } = new List<Function>();

	public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

	public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
