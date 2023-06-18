using ShopEF.EF;

public static class Programm
{
    static void Main(string[] args)
    {
        try
        {
            string old = "Data Source=D:\\Source\\ShopDB.db";
            string newp = "Data Source=D:\\Source\\ShopEF\\ShopEF\\ShopDB.db";
            DB_EF.CopyDB(old, newp);


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



