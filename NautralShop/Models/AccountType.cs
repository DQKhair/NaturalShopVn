using System;
using System.Collections.Generic;

namespace NautralShop.Models;

public partial class AccountType
{
    public int AccountTypeId { get; set; }

    public string AccountTypeName { get; set; } = null!;

    public virtual ICollection<Function> Functions { get; set; } = new List<Function>();
}
