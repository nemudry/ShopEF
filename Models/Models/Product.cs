﻿namespace ShopEF.Models;

public class Product : EntityBase
{
    public string Name { get; private set; } = null!;
    public string? Category { get; private set; }
    public string? Description { get; private set; }
    public string? Made { get; private set; }
    public double Price { get; private set; }

    public Discount? Discount { get; set; }
    public MscStorehouse? MscStorehouse { get; set; }
    public NnStorehouse? NnStorehouse { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Product(string name, string? category, string? description, string? made, double price)
    {
        Name = name;
        Category = category;
        Description = description;
        Made = made;
        Price = price;
    }

    public void ProductInfo()
    {
        Color.Cyan("Характеристики выбранного товара:");
        Console.WriteLine($"{Description}");
        Console.WriteLine($"Cтрана-производитель - {Made}.");
        Color.GreenShort($"Цена - {TotalPrice()}");
        if (Discount is not null) Color.Green($" - СКИДКА {Discount.Disc}% !!!.");
        else Console.WriteLine();
    }

    public double TotalPrice() => Price - Price * ((double)(Discount?.Disc ?? 0) / 100);    
}
