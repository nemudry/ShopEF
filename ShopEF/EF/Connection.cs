using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
namespace ShopEF.EF;

internal static class Connection
{
    //получение строки подключения и провайдера
    internal static void GetConnection(out string provider, out string connectionString)
    {
        //получение конфигурационного файла
        var config = GetConfiguration();
        //получение провайдера и строки подключения из конфигурационного файла
        provider = config["Provider"];
        connectionString = null;
        if (provider == "SQLite") connectionString = config.GetConnectionString("SQLiteConnection");
        if (provider == "SqlServer") connectionString = config.GetConnectionString("SQLServerConnection");       
    }

    //получение конфигурационного файла
    internal static IConfigurationRoot GetConfiguration ()
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("AppConfiguration.json");
        return builder.Build();
    }
}
