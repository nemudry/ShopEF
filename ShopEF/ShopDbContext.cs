using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShopEF;

public partial class ShopDbContext : DbContext
{
    public ShopDbContext()
    {
    }

    public ShopDbContext(DbContextOptions<ShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<MscStorehouse> MscStorehouses { get; set; }

    public virtual DbSet<NnStorehouse> NnStorehouses { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=ShopDB.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasIndex(e => e.Login, "IX_Clients_Login").IsUnique();
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_Discounts_ProductId").IsUnique();

            entity.Property(e => e.Discount1)
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
            entity.Property(e => e.DateOrder).HasDefaultValueSql("datetime('now')");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders).HasForeignKey(d => d.ClientId);

            entity.HasOne(d => d.Product).WithMany(p => p.Orders).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Products_Name").IsUnique();

            entity.Property(e => e.Category).HasDefaultValueSql("'Категория неизвестна'");
            entity.Property(e => e.Description).HasDefaultValueSql("'Описание отсутствует'");
            entity.Property(e => e.Made).HasDefaultValueSql("'Производитель неизвестен'");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
