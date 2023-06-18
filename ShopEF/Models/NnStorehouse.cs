namespace ShopEF.Models;

internal class NnStorehouse : EntityBase
{
    internal int ProductId { get; set; }

    internal int? ProductCount { get; set; }

    internal Product Product { get; set; } = null!;

    internal NnStorehouse (int productId, int? productCount)
    {
        ProductId = productId;
        ProductCount = productCount;
    }
}
