namespace ShopEF.ModelExtensions;

internal class AccountShop : Account
{
    internal Busket Busket { get; }
    internal purchaseStatus PurchaseStatus { get; set; }
    internal enum purchaseStatus { НоваяПокупка, ПродуктыВкорзине, ЗакончитьПокупку }
    internal clientStatus ClientStatus { get; set; }
    internal enum clientStatus { Авторизован, Аноним }

    //основной конструктор для работы в приложении
    internal AccountShop(int id, string fullName, string login, string clientPassword, List<Order> orders, purchaseStatus purchaseStatus, clientStatus clientStatus, Busket busket)
        :base(fullName, login, clientPassword)
    {
        Id = id;
        Orders = orders;
        Busket = busket;
        PurchaseStatus = purchaseStatus;
        ClientStatus = clientStatus;
    }
    //гостевой аккаунт
    internal AccountShop()
     : this(0, "Гость", "", "", new List<Order>(), purchaseStatus.НоваяПокупка, clientStatus.Аноним, new Busket())
    { }
    //деавторизация
    internal AccountShop(purchaseStatus purchaseStatus, Busket busket)
    : this(0, "Гость", "", "", new List<Order>(), purchaseStatus, clientStatus.Аноним, busket)
    { }
    //авторизация
    internal AccountShop(Account account, purchaseStatus purchaseStatus, Busket busket)
        : this(account.Id, account.FullName, account.Login, account.ClientPassword, account.Orders.ToList(), purchaseStatus, clientStatus.Авторизован, busket)
    { }

    // показать историю заказов
    internal void HistoryOrdersInfo()
    {
        if (Orders.Count != 0)
        {
            Console.Clear();
            Color.Green("История заказов:");
            Console.WriteLine();

            //номера заказов (даты)
            var idOrders = Orders.Select(e => e.DateOrder).Distinct();

            int numberOrder = 1;
            double price = 0;
            foreach (var idOrder in idOrders)
            {
                Console.WriteLine($"[{numberOrder}]. {Id}-{idOrder} ");
                foreach (var order in Orders)
                {
                    if (idOrder == order.DateOrder)
                    {
                        Console.WriteLine($"{order.Product.Name} - {order.CountProduct}шт.");
                        price += order.Price;
                    }
                }
                Console.WriteLine($"Цена заказа - {price}р.");
                Console.WriteLine();
                price = 0;
                numberOrder++;
            }
        }
        else
        {
            Color.Red("Заказы отсутствуют!");
            Console.WriteLine();
        }
    }
}
