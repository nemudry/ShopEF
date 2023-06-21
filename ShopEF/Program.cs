global using ShopEF.EF;
global using ShopEF.Models;
global using TechClasses;


namespace ShopEF;

public static class Programm
{
    static void Main(string[] args)
    {
        try
        {

            Shop shop = new ShopNN();
            shop.StartShop();



            /*
                       string old = "Data Source=D:\\Source\\ShopDB.db";
                       string newp = "Data Source=D:\\Source\\ShopEF\\ShopEF\\ShopDB.db";
                       var dd = DB_EF.CopyDB(old, newp);
                       dd.Wait();

                       */
            /*   using (var db = new ShopDbContext ())
               {

               }
               */


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



