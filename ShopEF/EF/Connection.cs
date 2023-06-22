using Microsoft.Extensions.Configuration;

namespace ShopEF.EF;
internal static class Connection
{
    internal static void GetConnectionString(out string provider, out string connectionString)
    {
        //получение конфигурационного файла
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("AppConfiguration.json");
        var config = builder.Build();

        //получение провайдера и строки подключения
        provider = config["Provider"];
        connectionString = null;
        if (provider == "SQLite") connectionString = config.GetConnectionString("SQLiteConnection");
        if (provider == "SqlServer") connectionString = config.GetConnectionString("SQLServerConnection");       
    }
}
