namespace ShopEF.EF;
internal static class Connection
{
    internal static void GetConnection(out string provider, out string connectionString)
    {
        //получение конфигурационного файла
        var config = GetConfiguration();

        //получение провайдера и строки подключения
        provider = config["Provider"];
        connectionString = null;
        if (provider == "SQLite") connectionString = config.GetConnectionString("SQLiteConnection");
        if (provider == "SqlServer") connectionString = config.GetConnectionString("SQLServerConnection");       
    }

    internal static IConfigurationRoot GetConfiguration ()
    {
        //получение конфигурационного файла
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("AppConfiguration.json");
        return builder.Build();
    }
}
