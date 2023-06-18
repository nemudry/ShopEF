using System;
using System.Collections.Generic;

namespace ShopEF;

public partial class Order
{
    public long Id { get; set; }

    public string? DateOrder { get; set; }

    public long ClientId { get; set; }

    public long ProductId { get; set; }

    public long CountProduct { get; set; }

    public long Price { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
