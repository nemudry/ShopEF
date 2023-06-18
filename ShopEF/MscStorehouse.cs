namespace ShopEF;

public partial class MscStorehouse
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public long? ProductCount { get; set; }

    public virtual Product Product { get; set; } = null!;
}
