using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ShopEF.Models;

//клиент
public class Account : EntityBase, IValidatableObject
{
    //имя клиента
    public string FullName { get; private set; } = null!;
    //логин
    public string Login { get; private set; } = null!;
    //пароль
    public string ClientPassword { get; private set; } = null!;
    //заказы
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Account(string fullName, string login, string clientPassword)
    {
        FullName = fullName;
        Login = login;
        ClientPassword = clientPassword;
    }

    //самовалидация модели
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(FullName))
            errors.Add(new ValidationResult("Не указано имя."));
        if (FullName.Length < 3 || FullName.Length > 20)
            errors.Add(new ValidationResult("Некорректная длина имени."));
        Regex regex = new Regex(@"[\d]");
        if (regex.IsMatch(FullName))
            errors.Add(new ValidationResult("В имени не могут присутствовать цифры."));

        if (string.IsNullOrWhiteSpace(Login))
            errors.Add(new ValidationResult("Не указан логин."));
        if (Login.Length < 3 || Login.Length > 20)
            errors.Add(new ValidationResult("Некорректная длина логина."));

        if (string.IsNullOrWhiteSpace(ClientPassword))
            errors.Add(new ValidationResult("Не указан пароль."));
        if (ClientPassword.Length < 3 || ClientPassword.Length > 20)
            errors.Add(new ValidationResult("Некорректная длина пароля."));

        return errors;
    }
}
