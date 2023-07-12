
namespace ShopEF.EF;
internal static class EFDatabase
{
    private readonly static ShopDbContext _db;

    static EFDatabase()
    {
        _db = new ShopDbContext(); 
    }

    //метод для переноса однотипных данных с одной бд на другую
    internal static async Task CopyDBAsync(string pathOldDb, string pathNewDb, string provOld = "SQLite", string provNew = "SQLite")
    {
        List<Product> products;
        List<Discount> discounts;
        List<MscStorehouse> mscStorehouse;
        List<NnStorehouse> nnStorehouse;
        List<Account> clients;
        List<Order> orders;

        try
        {
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
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при копировании базы данных!");
            Exceptions.ShowExInfo(e);
            Feedback.ReadKey();
        }
    }

    //Загрузка продуктов в магазин
    internal static async Task<Dictionary<Product, int>> LoadProductsAsync()
    {
        //продукты
        Dictionary<Product, int> ProductsInShop = new Dictionary<Product, int>();

        try
        {
            using (_db)
            {
                //продукты включая таблицы скидки, количества товара на двух складах
                var products = (_db.Products.Include(p => p.Discount).
                   Include(p => p.MscStorehouse).
                   Include(p => p.NnStorehouse)).ToListAsync().Result;

                foreach (var product in products)
                {
                    ProductsInShop.Add(product, (int)(product.NnStorehouse.ProductCount + product.MscStorehouse.ProductCount));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при загрузке товаров в магазин!");
            Exceptions.ShowExInfo(e);
            Feedback.ReadKey();
        }
        return ProductsInShop;
    }

    //проверка наличия клиента в бд
    internal static async Task<bool> CheckClientAsync(string login, string password = null)
    {
        Account? acc = null;
        try
        {
            using (_db)
            {
                //проверка только по логину
                if (password == null)
                {
                    acc = await _db.Clients.Where(c => c.Login == login).FirstOrDefaultAsync();
                }
                //проверка по логину/паролю
                else
                {
                    acc = await _db.Clients.Where(c => c.Login == login && c.ClientPassword == password).FirstOrDefaultAsync();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при проверке клиента в базе данных!");
            Exceptions.ShowExInfo(e);
            Feedback.ReadKey();
        }
        return acc is null ? false : true;
    }

    //получение клиента из БД
    internal static async Task<Account> GetClientAsync(string login, string password)
    {
        Account? acc = null;
        try
        {
            using (_db)
            {
                acc = await _db.Clients.Include(c => c.Orders).ThenInclude(o => o.Product). //с заказами 
                        Where(c => c.Login == login && c.ClientPassword == password).FirstOrDefaultAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при проверке клиента в базе данных!");
            Exceptions.ShowExInfo(e);
            Feedback.ReadKey();
        }
        return acc;
    }

    //регистрация нового клиента
    internal static bool SetNewClient(string fullName, string login, string password)
    {
        var result = false;
        try
        {
            using (_db)
            {
                _db.Clients.Add(new Account(fullName, login, password));
                _db.SaveAsy().Wait();
                result = true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при создании клиента в базе данных!");
            Exceptions.ShowExInfo(e);
            Feedback.ReadKey();
        }
        return result;
    }

    //формирование заказа в БД
    internal static async Task SetOrderAsync(DateTime dateTimeOrder, Account account, Dictionary<Product, int> purchase)
    {
        try
        {
            using (_db)
            {
                foreach (var product in purchase)
                {
                    _db.Orders.Add(new Order(dateTimeOrder, account.Id, product.Key.Id, product.Value, product.Key.TotalPrice() * product.Value));
                }
                await _db.SaveAsy();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при формировании заказа в базе данных!");
            Exceptions.ShowExInfo(e);
            Feedback.ReadKey();
        }
    }

    //Покупка товара (уменьшение количества товара на складах в БД)
    internal static async Task SetBuyProductsAsync(Dictionary<Product, int> purchase)
    {
        try
        {
            using (_db)
            {
                foreach (var product in purchase)
                {
                    //количество продукта на складах
                    var productNN = await _db.NnStorehouses.Where(p => p.ProductId == product.Key.Id).FirstOrDefaultAsync();
                    var productMSK = await _db.MscStorehouses.Where(p => p.ProductId == product.Key.Id).FirstOrDefaultAsync();

                    if (productNN!.ProductCount >= product.Value)
                    {
                        productNN.ProductCount -= product.Value; //если на ближайшем складе товара достаточно, берется только с него
                    }
                    else
                    {
                        productMSK!.ProductCount -= product.Value - productNN.ProductCount; // со второго склада MSC добирается остаток
                        productNN.ProductCount -= productNN.ProductCount; // c первого берется все продукция         
                    }
                }
                await _db.SaveAsy();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при проверке клиента в базе данных!");
            Exceptions.ShowExInfo(e);
            Feedback.ReadKey();
        }
    }
}
