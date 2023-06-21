namespace ShopEF.Models;

internal partial class Product : EntityBase
{
    internal string Name { get; set; } = null!;

    internal string? Category { get; set; }

    internal string? Description { get; set; }

    internal string? Made { get; set; }

    internal double Price { get; set; } 
    
    internal Discount? Discount { get; set; }

    internal MscStorehouse? MscStorehouse { get; set; }

    internal NnStorehouse? NnStorehouse { get; set; }

    internal ICollection<Order> Orders { get; set; } = new List<Order>();

    internal Product(string name, string? category, string? description, string? made, double price)
    {
        Name = name;
        Category = category;
        Description = description;
        Made = made;
        Price = price;
    }

    internal void ProductInfo()
    {
        Color.Cyan("Характеристики выбранного товара:");
        Console.WriteLine($"{Description}");
        Console.WriteLine($"Cтрана-производитель - {Made}.");
        Color.GreenShort($"Цена - {TotalPrice()}");
        if (Discount is not null) Color.Green($" - СКИДКА {Discount.Disc}% !!!.");
        else Console.WriteLine();
    }

    internal double TotalPrice() => Price - Price * ((double)(Discount?.Disc ?? 0) / 100);
    
}
