namespace ShopEF.Models;

//продукт
public class Product : EntityBase
{
    //название
    public string Name { get; private set; } = null!;
    //категория товара
    public string? Category { get; private set; }
    //описание
    public string? Description { get; private set; }
    //производитель
    public string? Made { get; private set; }
    //цена
    public double Price { get; private set; }

    //скидка
    public Discount? Discount { get; set; }
    //наличие товара на двух складах - москва и нижний
    public MscStorehouse? MscStorehouse { get; set; }
    public NnStorehouse? NnStorehouse { get; set; }
    //товар в заказах
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Product(string name, string? category, string? description, string? made, double price)
    {
        Name = name;
        Category = category;
        Description = description;
        Made = made;
        Price = price;
    }    
}
