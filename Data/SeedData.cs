// Data/SeedData.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyShopApi.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopApi.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
            await context.Database.MigrateAsync();

            if (context.Products.Any())
                return; // уже есть данные

            // Товары
            context.Products.AddRange(
                new Product { Name = "Ноутбук", Price = 1200, Stock = 10 },
                new Product { Name = "Мышь", Price = 25, Stock = 50 },
                new Product { Name = "Клавиатура", Price = 80, Stock = 30 }
            );

            // Покупатели
            context.Customers.AddRange(
                new Customer { FullName = "Иван Иванов", Email = "ivan@example.com" },
                new Customer { FullName = "Мария Петрова", Email = "maria@example.com" }
            );

            await context.SaveChangesAsync();
        }
    }
}