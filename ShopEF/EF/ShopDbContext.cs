using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ShopEF.EF;

//контекст бд
public class ShopDbContext : DbContext
{
    //логирование
    private readonly StreamWriter logsStream = new StreamWriter("ShopLogs.txt", true);
    //строка подключения
    private readonly string ConnectionString; 
    //провайдер
    private readonly string Provider;

    public ShopDbContext()
    {        
        //получение данных из конфигурационного файла
        Connection.GetConnection(out Provider, out ConnectionString);
    }
    public ShopDbContext(string provider, string path)
    {
        ConnectionString = path;
        Provider = provider;
    }

    //таблицы бд
    internal DbSet<Account> Clients { get; set; }
    internal DbSet<Discount> Discounts { get; set; }
    internal DbSet<MscStorehouse> MscStorehouses { get; set; }
    internal DbSet<NnStorehouse> NnStorehouses { get; set; }
    internal DbSet<Order> Orders { get; set; }
    internal DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //установка провайдера
        if (Provider == "SQLite") optionsBuilder.UseSqlite(ConnectionString);
        if (Provider == "SqlServer") optionsBuilder.UseSqlServer(ConnectionString);

        //логирование на консоль и в файл
        optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Information);
        optionsBuilder.LogTo(logsStream.WriteLine, LogLevel.Warning);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasIndex(e => e.Login, "IX_Clients_Login").IsUnique();
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_Discounts_ProductId").IsUnique();
            entity.Property(e => e.Disc)
                .HasDefaultValueSql("0")
                .HasColumnName("Discount");
            entity.HasOne(d => d.Product).WithOne(p => p.Discount).HasForeignKey<Discount>(d => d.ProductId);
        });

        modelBuilder.Entity<MscStorehouse>(entity =>
        {
            entity.ToTable("MSC_Storehouse");
            entity.HasIndex(e => e.ProductId, "IX_MSC_Storehouse_ProductId").IsUnique();
            entity.Property(e => e.ProductCount).HasDefaultValueSql("0");
            entity.HasOne(d => d.Product).WithOne(p => p.MscStorehouse).HasForeignKey<MscStorehouse>(d => d.ProductId);
        });

        modelBuilder.Entity<NnStorehouse>(entity =>
        {
            entity.ToTable("NN_Storehouse");
            entity.HasIndex(e => e.ProductId, "IX_NN_Storehouse_ProductId").IsUnique();
            entity.Property(e => e.ProductCount).HasDefaultValueSql("0");
            entity.HasOne(d => d.Product).WithOne(p => p.NnStorehouse).HasForeignKey<NnStorehouse>(d => d.ProductId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(d => d.Product).WithMany(p => p.Orders).HasForeignKey(d => d.ProductId);
            entity.HasOne(o => o.Account).WithMany(p => p.Orders).HasForeignKey(o => o.ClientId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Products_Name").IsUnique();
            entity.Property(e => e.Category).HasDefaultValueSql("'Категория неизвестна'");
            entity.Property(e => e.Description).HasDefaultValueSql("'Описание отсутствует'");
            entity.Property(e => e.Made).HasDefaultValueSql("'Производитель неизвестен'");
        });        
    }

    public override void Dispose()
    {
        base.Dispose();
        logsStream.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await logsStream.DisposeAsync();
    }

    //сохранение с ловлей ошибок
    internal async Task SaveAsy()
    {
        try
        {
            await SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            Console.WriteLine("Ошибка при сохранении DbUpdateConcurrencyException!");
            Console.WriteLine(e.Message);
            throw new Exception(e.Message);
        }
        catch (DbUpdateException e)
        {
            Console.WriteLine("Ошибка при сохранении DbUpdateException!");
            Console.WriteLine(e.Message);
            throw new Exception(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Другая ошибка при сохранении!");
            Console.WriteLine(e.Message);
            throw new Exception(e.Message);
        }
    }
}
