
namespace ShopEF;

internal abstract class Shop
{
    protected virtual string Name { get; }
    protected virtual string Description { get; }
    internal static Dictionary<Product, int> ProductsInShop { get; private set; }
    internal placeStatus PlaceInShop { get; set; }
    internal enum placeStatus
    {
        ВходВМагазин,
        ПереходНаГлавныйЭкран,
        ВКорзину
    }
    protected virtual AccountShop Account { get; set; }

    internal Shop()
    {
        Name = "";
        Description = "";
        PlaceInShop = placeStatus.ВходВМагазин;
        Account = new AccountShop();
        ProductsInShop = EFDatabase.LoadProductsAsync().Result;
    }

    //Запуск магазина
    public virtual void StartShop()
    {
        int answerWantToBuy = 0;
        while (true)
        {
            Console.Clear();
            PlaceInShop = placeStatus.ВходВМагазин;

            if (Account.PurchaseStatus == AccountShop.purchaseStatus.НоваяПокупка)
            {
                Color.Cyan($"Добро пожаловать в {Name}!");
                Color.Cyan($"{Description}");
                Console.WriteLine();

                answerWantToBuy = Validator.GetChechedAnswer("Хотите начать покупку?", 
                    new string[] { "Начать покупку", "Перейти в корзину", "Войти в личный кабинет" }, "Выйти из магазина");
            }

            // Если в корзине уже добавлен товар, начальное меню меняется
            // (продолжить покупку, а не начать покупку - пока не отоваришь корзину)
            if (Account.PurchaseStatus == AccountShop.purchaseStatus.ПродуктыВкорзине)
            {
                answerWantToBuy = Validator.GetChechedAnswer("Хотите продолжить покупку?", 
                    new string[] { "Продолжить покупку", "Перейти в корзину", "Войти в личный кабинет" }, "Выйти из магазина");
            }        

            //Начать покупку
            if (answerWantToBuy == 1) StartPurchase();

            // В Корзину
            if (answerWantToBuy == 2) GoToBusket();

            // Вход в аккаунт
            if (answerWantToBuy == 3)
            {
                if (CheckAuthorizationAsync().Result) GoToAccount();
            }

            // выход из программы
            if (answerWantToBuy == -1 || Account.PurchaseStatus == AccountShop.purchaseStatus.ЗакончитьПокупку)
            {
                Console.WriteLine("Всего доброго!");
                break;
            }
        }
    }


    
    //Начать покупку
    protected virtual void StartPurchase()
    {
        while (true)
        {
            // выход к начальному меню
            if (PlaceInShop == placeStatus.ПереходНаГлавныйЭкран) break;

            Console.Clear();

            // доступные категории
            var categories = ProductsInShop.Select(e => e.Key.Category).Distinct().ToList();

            // если товаров вообще нет
            if (categories.Count() == 0)
            {
                Color.Red("Все товары закончились. Вернитесь позже.");
                if (Account.PurchaseStatus != AccountShop.purchaseStatus.ПродуктыВкорзине)
                    Account.PurchaseStatus = AccountShop.purchaseStatus.ЗакончитьПокупку;

                AskAnswer.Accept();
                break;
            }

            Color.Cyan("Выберите категорию товара.");
            foreach (var category in categories)
            {
                Console.WriteLine($"[{categories.IndexOf(category) + 1}]. {category}");
            }
            Console.WriteLine();

            //выбор категории пользователем
            int answerIntCategory;
            while (true)
            {
                Color.Cyan("Введите категорию.");
                answerIntCategory = AskAnswer.AnswerPlayerInt();

                if (Validator.CheckСonditions(answerIntCategory, categories.Count, 1)) break;
            }

            // получение выбранной пользователем категории по номеру
            string chosenCategory = categories[answerIntCategory - 1];

            // переход к выбору товара из выбранной категории
            SelectProduct(chosenCategory);
        }
    }

    // выбор товара из выбранной категории
    protected void SelectProduct(string chosenCategory)
    {
        while (true)
        {
            if (PlaceInShop == placeStatus.ПереходНаГлавныйЭкран) break;

            Console.Clear();

            // получение доступных продуктов выбранной категории
            var productOfThisCategory = ProductsInShop.Where(e => e.Key.Category == chosenCategory).Select(e => e.Key).ToList();

            //вывод товаров выбранной категории на экран
            Color.Cyan($"Товары категории {chosenCategory}:");
            int numberOfProduct = 0;
            foreach (var product in productOfThisCategory)
            {
                numberOfProduct++;
                Console.Write($"{numberOfProduct}. {product.Name}, ");
                if (ProductsInShop[product] != 0)
                {
                    Console.Write($"количество на складе: {ProductsInShop[product]} шт.");
                    product.ShowDiscount();
                }
                else Color.Red($"отсутствует в наличии.");
            }
            Console.WriteLine();

            //выбор продукта для добавления в корзину
            int chosenProduct = 0;
            while (true)
            {
                Color.Cyan("Выберите дальнейшее действие.");
                Console.WriteLine("Для перехода на страничку товара введите его номер.");
                Console.WriteLine("Желаете вернуться к категориям - нажимите \"-1\".");
                Console.WriteLine("Желаете вернуться на главный экран - нажимите \"-2\".");
                chosenProduct = AskAnswer.AnswerPlayerInt();

                if (Validator.CheckСonditions(chosenProduct, productOfThisCategory.Count, 1, -1, -2)) break;
            };

            // назад в категории
            if (chosenProduct == -1) break;

            // назад на главный экран
            if (chosenProduct == -2)
            {
                PlaceInShop = placeStatus.ПереходНаГлавныйЭкран;
                break;
            }
            //определение товара
            else
            {
                int numberOfProduct2 = 1;
                foreach (var product in productOfThisCategory)
                {
                    if (numberOfProduct2 == chosenProduct)
                    {
                        Product selectedProduct = product;
                        //описание товара и добавление его в корзину
                        AddProductInBusket(selectedProduct);
                    }
                    numberOfProduct2++;
                }
            }
        }
    }

    //описание товара и добавление его в корзину
    protected void AddProductInBusket(Product product)
    {
        Console.Clear();

        // показ карточки товара
        product.ProductInfo();
        if (ProductsInShop[product] != 0)
        {
            Color.Green($"Количество на складе: {ProductsInShop[product]} шт.");
            Console.WriteLine();
        }
        else
        {
            Color.Red($"Отсутствует в наличии. Вернитесь позже.");
            AskAnswer.Accept();
            return;
        }

        int addToBusket;
        while (true)
        {
            Color.Cyan("Выберите дальнейшее действие:");
            Console.WriteLine($"1 - Добавить товар в корзину. \n2 - Вернуться к выбору товара.");
            addToBusket = AskAnswer.AnswerPlayerInt();

            if (Validator.CheckСonditions(addToBusket, 2, 1)) break;
        }

        // добавление товара в корзину
        if (addToBusket == 1)
        {
            int amountOfChosenProduct;
            //покупка "оптом"
            while (true)
            {
                Color.Cyan($"Сколько штук товара \"{product.Name}\" вы хотите добавить в корзину?");
                Color.Cyan($"На складе доступно \"{ProductsInShop[product]}\" шт.");
                amountOfChosenProduct = AskAnswer.AnswerPlayerInt();

                if (Validator.CheckСonditions(amountOfChosenProduct, ProductsInShop[product], 1)) break;
            }

            // добавление в корзину
            // если данный товар уже есть в корзине - добавить к нему количества, иначе добавить новый товар
            if (Account.Busket.ProductsInBusket.ContainsKey(product)) 
                Account.Busket.ProductsInBusket[product] += amountOfChosenProduct;
            else Account.Busket.ProductsInBusket.Add(product, amountOfChosenProduct);

            ProductsInShop[product] -= amountOfChosenProduct; // уменьшить количество товара в магазине
            Account.PurchaseStatus = AccountShop.purchaseStatus.ПродуктыВкорзине;
            PlaceInShop = placeStatus.ПереходНаГлавныйЭкран;

            Color.Green($"{amountOfChosenProduct} шт товара \"{product.Name}\" добавлено в корзину.");
            Console.WriteLine($"Стоимость всех товаров в корзине составляет {Account.Busket.TotalSum()}р.");
            AskAnswer.Accept();
        }
        // 2 - Вернуться к выбору товара.
        else return;
    }

    // переход в корзину 
    protected void GoToBusket()
    {
        while (true)
        {
            // если покупка совершена - выход в начальный цикл                
            if (PlaceInShop == placeStatus.ПереходНаГлавныйЭкран) break;

            // если в корзине нет товаров - выход в начальный цикл
            if (!Account.Busket.ProductsInBusket.Any())
            {
                Color.Red("Корзина пуста! Для оформления покупки сперва добавьте товаров корзину.");
                AskAnswer.Accept();
                break;
            }

            Color.Cyan("Вы находитесь в корзине!");
            // информация о продуктах в корзине
            Account.Busket.BusketInfo();

            //выбор действия в корзине
            int answerInBusket;
            while (true)
            {
                Color.Cyan("Выберите дальнейшее действие:");
                Console.WriteLine("[1]. Перейти к оплате. \n[2]. Удалить товар из корзины. " +
                    "\n[-1]. Вернуться к покупкам. ");
                answerInBusket = AskAnswer.AnswerPlayerInt();

                if (Validator.CheckСonditions(answerInBusket, 2, 1, -1)) break;
            }

            //Перейти к оплате.
            if (answerInBusket == 1)
            {
                if (CheckAuthorizationAsync().Result)
                {
                    PayPayment().Wait();
                }
                if (!Account.Busket.ProductsInBusket.Any()) PlaceInShop = placeStatus.ПереходНаГлавныйЭкран;
            }
            // удалить товар из корзины
            else if (answerInBusket == 2)
            {
                DeleteProductFromBusket();
            }
            // -1 вернуться к покупкам
            else break;
        }
    }

    //удаление товара из корзины
    protected void DeleteProductFromBusket()
    {
        while (true)
        {
            // информация о продуктах в корзине
            Account.Busket.BusketInfo();

            //выбор товара на удаление из корзины
            int answerIntRemoveProduct;
            while (true)
            {
                Color.Cyan("Введите номер товара, который вы хотите удалить: ");
                Color.Cyan("[-1]. Вернуться в корзину.");
                answerIntRemoveProduct = AskAnswer.AnswerPlayerInt();

                if (Validator.CheckСonditions(answerIntRemoveProduct, Account.Busket.ProductsInBusket.Count(), 1)) break;

                //[-1]. Вернуться в корзину.");
                if (answerIntRemoveProduct == -1) return;
            }

            // получение списка продуктов, определение удаляемого товара
            List<Product> productsList = new List<Product>();
            foreach (var product in Account.Busket.ProductsInBusket)
            {
                productsList.Add(product.Key);
            }
            Product deleteProduct = productsList[answerIntRemoveProduct - 1];

            // получение количества продукта на удаление
            int removeAmount;
            while (true)
            {
                Color.Cyan("Введите количество товара, который вы хотите удалить: ");
                removeAmount = AskAnswer.AnswerPlayerInt();

                if (Validator.CheckСonditions(removeAmount, Account.Busket.ProductsInBusket[deleteProduct], 1)) break;
            }

            //удаление товара
            // если удаляется не все количество товара в корзине - уменьшить количество, иначе удалить товар полностью
            if (Account.Busket.ProductsInBusket[deleteProduct] > removeAmount) Account.Busket.ProductsInBusket[deleteProduct] -= removeAmount;
            else Account.Busket.ProductsInBusket.Remove(deleteProduct);

            ProductsInShop[deleteProduct] += removeAmount;// добавление товара на полки магазина

            Color.Green($"{removeAmount} шт. товара \"{deleteProduct.Name}\" удалено из корзины.");
            Console.WriteLine();

            // если из корзины удалены все товары
            if (Account.Busket.ProductsInBusket.Count() == 0)
            {
                Account.PurchaseStatus = AccountShop.purchaseStatus.НоваяПокупка;
                PlaceInShop = placeStatus.ПереходНаГлавныйЭкран;
                break;
            }
        }
    }
    
    //проверка на авторизацию
    protected virtual async Task<bool> CheckAuthorizationAsync()
    {
        //если НЕавторизован
        while (true)
        {
            if (Account.ClientStatus == AccountShop.clientStatus.Аноним)
            {
                Color.Red("Вы не авторизованы!");
                Console.WriteLine();

                //выбор действия в аккаунте
                int answerInAccount;
                while (true)
                {
                    Color.Cyan("Выберите дальнейшее действие:");
                    Console.WriteLine("[1]. Регистрация. \n[2]. Авторизация. " +
                        "\n[-1]. Вернуться к покупкам. ");
                    answerInAccount = AskAnswer.AnswerPlayerInt();

                    if (Validator.CheckСonditions(answerInAccount, 2, 1, -1)) break;
                }

                //регистрация
                if (answerInAccount == 1)
                {
                    await RegistrationAsync();
                    break;
                }
                // авторизация
                else if (answerInAccount == 2)
                {
                    await AuthorizationAsync();         
                }
                // -1 вернуться к покупкам
                else return false;
            }
            else return true;
        }
        return false;
    }

    //регистрация
    protected virtual async Task RegistrationAsync()
    {
        while (true)
        {
            Console.Clear();

            //введите ФИО
            string answerFullName;
            while (true)
            {
                Color.Cyan("Введите ФИО: ");
                Color.Cyan("Для возврата к покупкам нажмите [-1]: ");
                answerFullName = AskAnswer.AnswerPlayerString();

                if (Validator.CheckСonditionsString(answerFullName)) break;
            }

            //выход из регистрации
            if (answerFullName == "-1") break;

            //введите логин
            string answerLogin;
            while (true)
            {
                Color.Cyan("Придумайте логин: ");
                answerLogin = AskAnswer.AnswerPlayerString();

                if (Validator.CheckСonditionsString(answerLogin)) break;
            }

            //введите пароль
            string answerPassword;
            while (true)
            {
                Color.Cyan("Придумайте пароль: ");
                answerPassword = AskAnswer.AnswerPlayerString();

                if (Validator.CheckСonditionsString(answerPassword)) break;
            }

            //Проверка на наличие данного клиента в бд        
            bool isHasClint = await EFDatabase.CheckClientAsync(answerLogin);

            //Если клиента нет в бд - регистрация нового клиента
            if (!isHasClint)
            {
                var result = EFDatabase.SetNewClient(answerFullName, answerLogin, answerPassword);
                if (result) Color.Green("Регистрация прошла успешно!");
                else Color.Red("Регистрация не прошла!");
                AskAnswer.Accept();
                break;
            }
            //Если логин/пароль заняты
            else
            {
                Color.Red("Введенный логин занят!"); 
                AskAnswer.Accept();
            }
        }
    }

    //авторизация
    protected virtual async Task AuthorizationAsync()
    {
        while (true)
        {
            Console.Clear();

            //введите логин
            string answerLogin;
            while (true)
            {
                Color.Cyan("Введите логин: ");
                Color.Cyan("Для возврата к покупкам нажмите [-1]: ");
                answerLogin = AskAnswer.AnswerPlayerString();

                if (Validator.CheckСonditionsString(answerLogin)) break;
            }

            //выход из авторизации
            if (answerLogin == "-1") break;

            //введите пароль
            string answerPassword;
            while (true)
            {
                Color.Cyan("Введите пароль: ");
                answerPassword = AskAnswer.AnswerPlayerString();

                if (Validator.CheckСonditionsString(answerPassword)) break;
            }

            //Проверка на наличие данного клиента в бд
            bool isHasClint = await EFDatabase.CheckClientAsync(answerLogin, answerPassword);    

            //Если клиент есть в бд - авторизация
            if (isHasClint)
            {
                Account = new AccountShop (await EFDatabase.GetClientAsync(answerLogin, answerPassword), Account.PurchaseStatus, Account.Busket);
                Color.Green("Авторизация прошла успешно!");
                AskAnswer.Accept();
                break;
            }
            //Если логин/пароль не подходят
            else
            {
                Color.Red("Введенные логин/пароль не подходят!");
                AskAnswer.Accept();
            }
        }
    }
   
    //деавторизация
    protected virtual void Deauthorization()
    {
        Account = new AccountShop (Account.PurchaseStatus, Account.Busket);

        Color.Green("Выход из аккаунта произведен успешно!");
        AskAnswer.Accept();
    }

    // переход в аккаунт 
    protected void GoToAccount()
    {
        Console.Clear();

        while (true)
        {
            Color.Cyan("Вы находитесь в личном кабинете!");
            Color.Cyan($"ФИО: {Account.FullName}");

            //выбор действия в корзине
            int answerInAccount;
            while (true)
            {
                Color.Cyan("Выберите дальнейшее действие:");
                Console.WriteLine("[1]. Посмотреть историю заказов. \n[2]. Деавторизоваться. " +
                    "\n[-1]. Вернуться к покупкам. ");
                answerInAccount = AskAnswer.AnswerPlayerInt();

                if (Validator.CheckСonditions(answerInAccount, 2, 1, -1)) break;
            }

            //История заказов.
            if (answerInAccount == 1)
            {
                Account.HistoryOrdersInfo();
            }
            // деавторизация
            else if (answerInAccount == 2)
            {
                Deauthorization();
                break;
            }
            // -1 вернуться к покупкам
            else break;
        }
    }

    // Оплата товара
    public async Task PayPayment()
    {
        int answerPayment;
        while (true)
        {
            Console.Clear();

            // выберите способ оплаты
            while (true)
            {
                Console.WriteLine($"Стоимость всех товаров в корзине составляет {Account.Busket.TotalSum()}р.");
                Color.Cyan("Выберите способ оплаты: ");
                Console.WriteLine("[1]. Оплата по карте. \n[-1]. Вернуться в корзину.");
                answerPayment = AskAnswer.AnswerPlayerInt();

                if (Validator.CheckСonditions(answerPayment, 1, 1, -1)) break;
            }

            //3. Оплата по карте.
            if (answerPayment == 1)
            {
                //формирование заказа в бд
                await EFDatabase.SetOrderAsync(DateTime.Now, Account, Account.Busket.ProductsInBusket);

                //покупка товаров(уменьшение товара на складах)
                await EFDatabase.SetBuyProductsAsync(Account.Busket.ProductsInBusket);

                Color.Green($"Денежные средства в размере {Account.Busket.TotalSum()}р списаны с Вашей банковской карты. Благодарим за покупку!");
                AskAnswer.Accept();

                // очистить корзину
                Account.Busket.ProductsInBusket.Clear();

                Account.PurchaseStatus = AccountShop.purchaseStatus.НоваяПокупка;
                break;
            }

            // Вернуться в корзину.
            if (answerPayment == -1) break;
        }
    }
}
