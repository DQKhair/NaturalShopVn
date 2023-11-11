using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class Employee
{
    public string EmployeeId { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string? EmployeeAddress { get; set; }

    public string? EmployeeEmail { get; set; }

    public string? EmployeePhone { get; set; }

    public string? EmployeeUsername { get; set; }

    public string? EmployeePassword { get; set; }

    public bool EmployeeStatus { get; set; }

    public int? FunctionId { get; set; }

    public virtual Function? Function { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
