namespace ShopEF.Models;

internal class Discount : EntityBase
{
    internal int ProductId { get; set; }

    internal double? Disc { get; set; }

    internal Product Product { get; set; } = null!;

    internal Discount(int productId, double? disc)
    {
        ProductId = productId;
        Disc = disc;
    }
}
