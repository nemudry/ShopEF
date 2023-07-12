
namespace ShopEF;
public static class Programm
{
    static void Main(string[] args)
    {
        try
        {
            Shop shop = new ShopNN();
            shop.StartShop();
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка!");
            Exceptions.ShowExInfo(e);
        }
    }     
}



