using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEF.Models;

internal class Order : EntityBase
{
    internal DateTime DateOrder { get; set; }

    private int ClientId { get; set; }

    internal int ProductId { get; set; }

    internal int CountProduct { get; set; }

    internal double Price { get; set; }

    [ForeignKey("ClientId")]
    private Account Account { get; set; } = null!;

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
