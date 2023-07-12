
namespace ShopEF.ModelExtensions;
internal static class ProductExtention
{
    //показать информацию о магазине
    internal static void ProductInfo(this Product product)
    {
        Color.Cyan("Характеристики выбранного товара:");
        Console.WriteLine($"{product.Description}");
        Console.WriteLine($"Cтрана-производитель - {product.Made}.");
        Color.GreenShort($"Цена - {product.TotalPrice()}");
        product.ShowDiscount();
    }

    //цена продукта со скидкой
    internal static double TotalPrice(this Product product)
    {
        if (product.Discount == null) return product.Price;
        else return product.Price - product.Price * ((double)product.Discount.Disc / 100);
    }

    //показать скидку
    internal static void ShowDiscount(this Product product)
    {
        if (product.Discount is not null) Color.Green($" - СКИДКА {product.Discount.Disc}% !!!.");
        else Console.WriteLine();
    }
}
