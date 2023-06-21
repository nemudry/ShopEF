using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEF.Models;

[Index("Login", IsUnique = true, Name = "IX_Clients_Login")]
internal class Account : EntityBase
{
    internal string FullName { get; set; } = null!;
    internal string Login { get; set; } = null!;
    internal string ClientPassword { get; set; } = null!;
    internal ICollection<Order> Orders { get; set; } = new List<Order>();

    [NotMapped]
    internal Busket Busket { get; }
    [NotMapped]
    internal purchaseStatus PurchaseStatus { get; set; }
    internal enum purchaseStatus { НоваяПокупка, ПродуктыВкорзине, ЗакончитьПокупку }
    [NotMapped]
    internal clientStatus ClientStatus { get; set; }
    internal enum clientStatus { Авторизован, Аноним }

    internal Account(int id, string fullName, string login, string clientPassword, List<Order> orders, purchaseStatus purchaseStatus, clientStatus clientStatus, Busket busket)
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
    internal Account()
     : this(0, "Гость", "", "", new List<Order>(), purchaseStatus.НоваяПокупка, clientStatus.Аноним, new Busket())
    {    }
    internal Account(purchaseStatus purchaseStatus, Busket busket)
    : this(0, "Гость", "", "", new List<Order>(), purchaseStatus, clientStatus.Аноним, busket)
    {    }
    internal Account(Account account, purchaseStatus purchaseStatus, Busket busket)
        : this (account.Id, account.FullName, account.Login, account.ClientPassword, account.Orders.ToList(), purchaseStatus, clientStatus.Авторизован, busket)
    {    }
    internal Account(string fullName, string login, string clientPassword)
    {
        FullName = fullName;
        Login = login;
        ClientPassword = clientPassword;
    }

    
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
            Color.Red("Заказы отсутсвуют!");
            Console.WriteLine();
        }
    }

    // Оплата товара
    internal async Task PayPayment()
    {
        int answerPayment;
        while (true)
        {
            Console.Clear();

            // выберите способ оплаты
            while (true)
            {
                Console.WriteLine($"Стоимость всех товаров в корзине составляет {Busket.TotalSum()}р.");
                Color.Cyan("Выберите способ оплаты: ");
                Console.WriteLine("[1]. Оплата по карте. \n[-1]. Вернуться в корзину.");
                answerPayment = Feedback.PlayerAnswer();

                if (Feedback.CheckСonditions(answerPayment, 1, 1, -1)) break;
            }

            //3. Оплата по карте.
            if (answerPayment == 1)
            {
                //формирование заказа в бд
                await DB_EF.SetOrderAsync(DateTime.Now, this, Busket.ProductsInBusket);

                //покупка товаров(уменьшение товара на складах)
                await DB_EF.SetBuyProductsAsync(Busket.ProductsInBusket);

                Color.Green($"Денежные средства в размере {Busket.TotalSum()}р списаны с Вашей банковской карты. Благодарим за покупку!");
                Feedback.ReadKey();

                // очистить корзину
                Busket.ProductsInBusket.Clear();

                PurchaseStatus = purchaseStatus.НоваяПокупка;
                break;
            }

            // Вернуться в корзину.
            if (answerPayment == -1) break;
        }
    }
}
