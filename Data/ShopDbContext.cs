using Microsoft.EntityFrameworkCore;
using MyShopApi.Models;
using System.Collections.Generic;

namespace MyShopApi.Data;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
}
