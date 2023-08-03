namespace ShopEF.Models;
public class Discount : EntityBase
{ 
    //продукт и его id
    public int ProductId { get; private set; }
    public Product Product { get; set; } = null!;
    // скидка
    public double? Disc { get; private set; }

    public Discount(int productId, double? disc)
    {
        ProductId = productId;
        Disc = disc;
    }
}
