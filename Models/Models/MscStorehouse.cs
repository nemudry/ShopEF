namespace ShopEF.Models;
//нижегородский склад
public class MscStorehouse : EntityBase
{    
    //продукт и его id
    public int ProductId { get; private set; }
    public Product Product { get; set; } = null!;
    //количество продукта
    public int? ProductCount { get; set; }

    public MscStorehouse(int productId, int? productCount)
    {
        ProductId = productId;
        ProductCount = productCount;
    }
}
