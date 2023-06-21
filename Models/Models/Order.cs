using System.Net.Sockets;

namespace ShopEF.Models;

public class Order : EntityBase
{
    public DateTime DateOrder { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int CountProduct { get; set; }
    public double Price { get; set; }

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
