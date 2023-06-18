using Microsoft.EntityFrameworkCore;
namespace ShopEF.Models;


[Index("Login", IsUnique = true, Name = "IX_Clients_Login")]
internal class Account : EntityBase
{
    private string FullName { get; set; } = null!;

    private string Login { get; set; } = null!;

    private string ClientPassword { get; set; } = null!;

    private ICollection<Order> Orders { get; set; } = new List<Order>();

    internal Account(string fullName, string login, string clientPassword)
    {
        FullName = fullName;
        Login = login;
        ClientPassword = clientPassword;
    }
}
