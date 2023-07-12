namespace ShopEF.Models;

public class Account : EntityBase
{
    public string FullName { get; private set; } = null!;
    public string Login { get; private set; } = null!;
    public string ClientPassword { get; private set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Account(string fullName, string login, string clientPassword)
    {
        FullName = fullName;
        Login = login;
        ClientPassword = clientPassword;
    }
}
