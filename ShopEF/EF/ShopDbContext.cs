using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ShopEF.EF;

public class ShopDbContext : DbContext
{
    private string ConnectionString ;

    public ShopDbContext()
    {
        ConnectionString = "Data Source=D:\\Source\\ShopEF\\ShopEF\\ShopDB.db";
        //  Database.EnsureDeleted();
        // Database.EnsureCreated();
    }

    public ShopDbContext(string path)
    {
        ConnectionString = path;
    }
    
    internal DbSet<Account> Clients { get; set; }
    internal DbSet<Discount> Discounts { get; set; }
    internal DbSet<MscStorehouse> MscStorehouses { get; set; }
    internal DbSet<NnStorehouse> NnStorehouses { get; set; }
    internal DbSet<Order> Orders { get; set; }
    internal DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(ConnectionString);
        optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Clients");

            entity.Property("FullName");
            entity.Property("ClientPassword");
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
            entity.Property("Price");
            entity.Property("CountProduct");

            entity.Property(e => e.DateOrder).HasDefaultValueSql("datetime('now')");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders).HasForeignKey(d => d.ProductId);
            entity.HasOne(o => o.Account).WithMany(p => p.Orders).HasForeignKey(o => o.ClientId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Products_Name").IsUnique();

            entity.Property("Price");

            entity.Property(e => e.Category).HasDefaultValueSql("'Категория неизвестна'");
            entity.Property(e => e.Description).HasDefaultValueSql("'Описание отсутствует'");
            entity.Property(e => e.Made).HasDefaultValueSql("'Производитель неизвестен'");
        });
    }
}
