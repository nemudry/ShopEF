namespace ShopEF.Models;

internal class Discount : EntityBase
{
    internal int ProductId { get; set; }

    internal int? Disc { get; set; }

    internal Product Product { get; set; } = null!;

    internal Discount(int productId, int? disc)
    {
        ProductId = productId;
        Disc = disc;
    }
}
