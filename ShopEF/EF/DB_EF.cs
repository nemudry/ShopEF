
using Microsoft.EntityFrameworkCore;

namespace ShopEF.EF;

internal static class DB_EF
{
    //метод для переноса однотипных данных с одной бд на другую
    internal static void CopyDB(string pathOldDb, string pathNewDb)
    {
        using (var dbOld = new ShopDbContext(pathOldDb))
        {
            dbOld.Clients.Load();
            dbOld.Products.Load();
            dbOld.Discounts.Load();
            dbOld.NnStorehouses.Load();
            dbOld.MscStorehouses.Load();
            dbOld.Orders.Load();

            using (var dbnew = new ShopDbContext(pathNewDb))
            {
                foreach (var client in dbOld.Clients)
                {
                    dbnew.Clients.Add(client);
                }
                foreach (var product in dbOld.Products)
                {
                    dbnew.Products.Add(product);
                }
                foreach (var discount in dbOld.Discounts)
                {
                    dbnew.Discounts.Add(discount);
                }
                foreach (var nnStorehouse in dbOld.NnStorehouses)
                {
                    dbnew.NnStorehouses.Add(nnStorehouse);
                }
                foreach (var mscStorehouses in dbOld.MscStorehouses)
                {
                    dbnew.MscStorehouses.Add(mscStorehouses);
                }
                foreach (var order in dbOld.Orders)
                {
                    dbnew.Orders.Add(order);
                }
                dbnew.SaveChanges();
            }
        }
    }
}
