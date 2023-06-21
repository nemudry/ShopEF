namespace ShopEF.Models;
public class NnStorehouse : EntityBase
{
    public int ProductId { get; set; }
    public int? ProductCount { get; set; }

    public Product Product { get; set; } = null!;

    public NnStorehouse (int productId, int? productCount)
    {
        ProductId = productId;
        ProductCount = productCount;
    }
}
