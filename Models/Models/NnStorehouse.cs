namespace ShopEF.Models;
//нижегородский склад
public class NnStorehouse : EntityBase
{
    //продукт и его id
    public int ProductId { get; private set; }
    public Product Product { get; set; } = null!;
    //количество продукта
    public int? ProductCount { get; set; }

    public NnStorehouse (int productId, int? productCount)
    {
        ProductId = productId;
        ProductCount = productCount;
    }
}
