using System;
using System.Collections.Generic;

namespace ShopEF;

public partial class Client
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string ClientPassword { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
