namespace ShopEF.Models;

internal class MscStorehouse : EntityBase
{
    internal int ProductId { get; set; }

    internal int? ProductCount { get; set; }

    internal Product Product { get; set; } = null!;

    internal MscStorehouse(int productId, int? productCount)
    {
        ProductId = productId;
        ProductCount = productCount;
    }
}
