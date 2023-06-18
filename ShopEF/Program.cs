using ShopEF.EF;

public static class Programm
{
    static void Main(string[] args)
    {
        try
        {
            
            using (var db = new ShopDbContext ())
            {

                Console.WriteLine("Hello, World!");
            }
            Console.WriteLine("Hello, World!");
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка!");
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.TargetSite);
            Console.WriteLine(e.Source);
            Console.WriteLine(e.InnerException?.Message);
        }
    }     
}



