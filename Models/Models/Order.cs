namespace ShopEF.Models;

//заказ
public class Order : EntityBase
{
    //дата и время заказа
    public DateTime DateOrder { get; private set; }
    //клиент-покупатель
    public int ClientId { get; private set; }
    public Account Account { get; set; } = null!;
    //купленный товар
    public int ProductId { get; private set; }
    public Product Product { get; set; } = null!;
    //количество товара
    public int CountProduct { get; private set; }
    //цена
    public double Price { get; private set; }

    public Order(DateTime dateOrder, int clientId, int productId, int countProduct, double price)
    {
        DateOrder = dateOrder;
        ClientId = clientId;
        ProductId = productId;
        CountProduct = countProduct;
        Price = price;
    }
}
