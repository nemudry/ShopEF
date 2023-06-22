using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ShopEF.EF;
internal static class DB_EF
{
    //метод для переноса однотипных данных с одной бд на другую
    internal static async Task CopyDBAsync(string pathOldDb, string pathNewDb, string provOld = "SQLite", string provNew = "SQLite")
    {
        List<Product> products;
        List<Discount> discounts;
        List<MscStorehouse> mscStorehouse;
        List<NnStorehouse> nnStorehouse;
        List<Account> clients;
        List<Order> orders;

        using (var dbOld = new ShopDbContext(provOld, pathOldDb))
        {
            products = await dbOld.Products.ToListAsync();
            discounts = await dbOld.Discounts.ToListAsync();
            mscStorehouse = await dbOld.MscStorehouses.ToListAsync();
            nnStorehouse = await dbOld.NnStorehouses.ToListAsync();
            clients = await dbOld.Clients.ToListAsync();
            orders = await dbOld.Orders.ToListAsync();
        }

        using (var dbnew = new ShopDbContext(provNew, pathNewDb))
        {
            Task[] tasks = new Task[6];
            tasks[0] = dbnew.Clients.AddRangeAsync(clients);
            tasks[1] = dbnew.Products.AddRangeAsync(products);
            tasks[2] = dbnew.Discounts.AddRangeAsync(discounts);
            tasks[3] = dbnew.NnStorehouses.AddRangeAsync(nnStorehouse);
            tasks[4] = dbnew.MscStorehouses.AddRangeAsync(mscStorehouse);
            tasks[5] = dbnew.Orders.AddRangeAsync(orders);

            Task.WaitAll(tasks);

            await dbnew.SaveAsy();
        }
    }    

    //Загрузка продуктов в магазин
    internal static async Task<Dictionary<Product, int>> LoadProductsAsync()
    {
        //продукты
        Dictionary<Product, int> ProductsInShop = new Dictionary<Product, int>();

        using (var db = new ShopDbContext())
        {
            //продукты включая таблицы скидки, количества товара на двух складах
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
    internal static async Task<bool> CheckClientAsync(string login, string password = null)
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
    internal static async Task<Account> GetClientAsync(string login, string password)
    {
        Account? acc;
        using (var db = new ShopDbContext())
        {
            acc = await db.Clients.Include(c => c.Orders).ThenInclude(o => o.Product). //с заказами 
                    Where(c => c.Login == login && c.ClientPassword == password).FirstOrDefaultAsync();    
        }
        return acc;
    }

    //регистрация нового клиента
    internal static void SetNewClient(string fullName, string login, string password)
    {        
        using (var db = new ShopDbContext())
        {
            db.Clients.Add(new Account(fullName, login, password));
            db.SaveAsy().Wait();
        }
    }

    //формирование заказа в БД
    internal static async Task SetOrderAsync(DateTime dateTimeOrder, Account account, Dictionary<Product, int> purchase)
    {
        using (var db = new ShopDbContext())
        {
            foreach (var product in purchase)
            {
                db.Orders.Add(new Order(dateTimeOrder, account.Id, product.Key.Id, product.Value, product.Key.TotalPrice() * product.Value));
            }
            await db.SaveAsy();
        }
    }

    //Покупка товара (уменьшение количества товара на складах в БД)
    internal static async Task SetBuyProductsAsync(Dictionary<Product, int> purchase)
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
            await db.SaveAsy();       
        }
    }
}
