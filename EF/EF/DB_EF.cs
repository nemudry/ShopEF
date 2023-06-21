
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ShopEF.EF;

public static class DB_EF
{

    //метод для переноса однотипных данных с одной бд на другую
    public static async Task CopyDBAsync(string pathOldDb, string pathNewDb)
    {
        using (var dbOld = new ShopDbContext(pathOldDb))
        {
            var products = dbOld.Products.ToListAsync();
            var discounts =dbOld.Discounts.ToListAsync();
            var mscStorehouse = dbOld.MscStorehouses.ToListAsync();
            var nnStorehouse = dbOld.NnStorehouses.ToListAsync();
            var clients = dbOld.Clients.ToListAsync();
            var orders = dbOld.Orders.ToListAsync();

            using (var dbnew = new ShopDbContext(pathNewDb))
            {
                Task[] tasks = new Task[6]; 
                tasks[0] = dbnew.Clients.AddRangeAsync(clients.Result);
                tasks[1] = dbnew.Products.AddRangeAsync(products.Result);
                tasks[2] = dbnew.Discounts.AddRangeAsync(discounts.Result);
                tasks[3] = dbnew.NnStorehouses.AddRangeAsync(nnStorehouse.Result);
                tasks[4] = dbnew.MscStorehouses.AddRangeAsync(mscStorehouse.Result);
                tasks[5] = dbnew.Orders.AddRangeAsync(orders.Result);

                Task.WaitAll(tasks);

                await dbnew.SaveChangesAsync();
            }
        }
    }

    //Загрузка продуктов в магазин
    public static async Task<Dictionary<Product, int>> LoadProductsAsync()
    {
        //продукты
        Dictionary<Product, int> ProductsInShop = new Dictionary<Product, int>();

        using (var db = new ShopDbContext())
        {
            var products = (db.Products.Include(p => p.Discount).
               Include(p => p.MscStorehouse).
               Include(p => p.NnStorehouse)).ToListAsync().Result;           

            foreach (var product in products)
            {   
                ProductsInShop.Add(product, (int) (product.NnStorehouse.ProductCount + product.MscStorehouse.ProductCount));                
            }
        }
        return ProductsInShop;
    }

    //проверка наличия клиента в бд
    public static async Task<bool> CheckClientAsync(string login, string password = null)
    {
        Account? acc;
        using (var db = new ShopDbContext())
        {
            //проверка только по логину
            if (password == null)
            {
                acc = await db.Clients.Where(c => c.Login == login).FirstOrDefaultAsync();
            }
            //проверка по логину/паролю
            else
            {
                acc = await db.Clients.Where(c => c.Login == login && c.ClientPassword == password).FirstOrDefaultAsync();               
            }
        }
        return acc is null ? false : true;
    }

    //получение клиента из БД
    public static async Task<Account> GetClientAsync(string login, string password)
    {
        Account? acc;
        using (var db = new ShopDbContext())
        {
            acc = await db.Clients.Include(c => c.Orders).ThenInclude(o => o.Product).
                    Where(c => c.Login == login && c.ClientPassword == password).FirstOrDefaultAsync();    
        }
        return acc;
    }

    //регистрация нового клиента
    public static void SetNewClient(string fullName, string login, string password)
    {        
        using (var db = new ShopDbContext())
        {
            db.Clients.Add(new Account(fullName, login, password));
            db.SaveChanges();
        }
    }

    //формирование заказа в БД
    public static async Task SetOrderAsync(DateTime dateTimeOrder, Account account, Dictionary<Product, int> purchase)
    {
        using (var db = new ShopDbContext())
        {
            foreach (var product in purchase)
            {
                db.Orders.Add(new Order(dateTimeOrder, account.Id, product.Key.Id, product.Value, product.Key.TotalPrice() * product.Value));
            }            
            await db.SaveChangesAsync();
        }
    }

    //Покупка товара (уменьшение количества товара на складах в БД)
    public static async Task SetBuyProductsAsync(Dictionary<Product, int> purchase)
    {
        using (var db = new ShopDbContext())
        {
            foreach (var product in purchase)
            {
                var productNN = await db.NnStorehouses.Where(p => p.ProductId == product.Key.Id).FirstOrDefaultAsync();
                var productMSK = await db.MscStorehouses.Where(p => p.ProductId == product.Key.Id).FirstOrDefaultAsync();

                if (productNN.ProductCount >= product.Value)
                {
                    productNN.ProductCount -= product.Value;
                }
                else
                {
                    productMSK.ProductCount -= product.Value - productNN.ProductCount; // со второго склада MSC добирается остаток
                    productNN.ProductCount -= productNN.ProductCount; // c первого берется все продукция         
                }
            }
            await db.SaveChangesAsync();       
        }
    }
}
