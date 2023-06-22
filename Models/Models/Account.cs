using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEF.Models;

public class Account : EntityBase
{
    public string FullName { get; private set; } = null!;
    public string Login { get; private set; } = null!;
    public string ClientPassword { get; private set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    [NotMapped]
    public Busket Busket { get; }
    [NotMapped]
    public purchaseStatus PurchaseStatus { get; set; }
    public enum purchaseStatus { НоваяПокупка, ПродуктыВкорзине, ЗакончитьПокупку }
    [NotMapped]
    public clientStatus ClientStatus { get; set; }
    public enum clientStatus { Авторизован, Аноним }

    //конструктор для загрузки модели из бд
    public Account(string fullName, string login, string clientPassword)
    {
        FullName = fullName;
        Login = login;
        ClientPassword = clientPassword;
    }
    //основной конструктор для работы в приложении
    public Account(int id, string fullName, string login, string clientPassword, List<Order> orders, purchaseStatus purchaseStatus, clientStatus clientStatus, Busket busket)
    {
        Id = id;
        FullName = fullName;
        Login = login;
        ClientPassword = clientPassword;
        Orders = orders;
        Busket = busket;
        PurchaseStatus = purchaseStatus;
        ClientStatus = clientStatus;
    }
    //гостевой аккаунт
    public Account()
     : this(0, "Гость", "", "", new List<Order>(), purchaseStatus.НоваяПокупка, clientStatus.Аноним, new Busket())
    {    }
    //деавторизация
    public Account(purchaseStatus purchaseStatus, Busket busket)
    : this(0, "Гость", "", "", new List<Order>(), purchaseStatus, clientStatus.Аноним, busket)
    {    }
    //авторизация
    public Account(Account account, purchaseStatus purchaseStatus, Busket busket)
        : this (account.Id, account.FullName, account.Login, account.ClientPassword, account.Orders.ToList(), purchaseStatus, clientStatus.Авторизован, busket)
    {    }

    // показать историю заказов
    public void HistoryOrdersInfo()
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
            Color.Red("Заказы отсутсвуют!");
            Console.WriteLine();
        }
    }
}
