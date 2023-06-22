namespace ShopEF.Models;
public class MscStorehouse : EntityBase
{
    public int ProductId { get; private set; }
    public int? ProductCount { get; set; }
    public Product Product { get; set; } = null!;

    public MscStorehouse(int productId, int? productCount)
    {
        ProductId = productId;
        ProductCount = productCount;
    }
}
