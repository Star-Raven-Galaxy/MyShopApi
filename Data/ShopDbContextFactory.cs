using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace MyShopApi.Data
{
    public class ShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
    {
        public ShopDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShopDbContext>();

            // Строка подключения по умолчанию для миграций
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
                                   ?? "Host=localhost;Database=myshop;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);

            return new ShopDbContext(optionsBuilder.Options);
        }
    }
}
