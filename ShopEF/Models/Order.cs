using System.Net.Sockets;

namespace ShopEF.Models;

internal class Order : EntityBase
{
    internal DateTime DateOrder { get; set; }

    internal int ClientId { get; set; }

    internal int ProductId { get; set; }

    internal int CountProduct { get; set; }

    internal double Price { get; set; }

    internal Account Account { get; set; } = null!;

    internal Product Product { get; set; } = null!;

    internal Order(DateTime dateOrder, int clientId, int productId, int countProduct, double price)
    {
        DateOrder = dateOrder;
        ClientId = clientId;
        ProductId = productId;
        CountProduct = countProduct;
        Price = price;
    }
}
