
namespace ShopEF.Models;

public class Order : EntityBase
{
    public DateTime DateOrder { get; private set; }
    public int ClientId { get; private set; }
    public int ProductId { get; private set; }
    public int CountProduct { get; private set; }
    public double Price { get; private set; }

    public Account Account { get; set; } = null!;
    public Product Product { get; set; } = null!;

    public Order(DateTime dateOrder, int clientId, int productId, int countProduct, double price)
    {
        DateOrder = dateOrder;
        ClientId = clientId;
        ProductId = productId;
        CountProduct = countProduct;
        Price = price;
    }
}
