// Services/ProductService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyShopApi.Data;
using MyShopApi.Models;

namespace MyShopApi.Services
{
    public class ProductService : IProductService
    {
        private readonly ShopDbContext _db;
        private readonly ICacheService _cache;
        private const string AllProductsKey = "products_all";
        private static string ProductKey(int id) => $"product_{id}";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public ProductService(ShopDbContext db, ICacheService cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // Временно отключаем кэш для отладки
            // return await _cache.GetOrSetAsync(AllProductsKey,
            //     async () => await _db.Products.ToListAsync(),
            //     CacheTtl);
            return await _db.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            // return await _cache.GetOrSetAsync(ProductKey(id),
            //     async () => await _db.Products.FindAsync(id),
            //     CacheTtl);
            return await _db.Products.FindAsync(id);
        }

        // В методах CreateAsync, UpdateAsync, DeleteAsync закомментируйте строки с _cache.RemoveAsync
        public async Task<Product> CreateAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            // await InvalidateCacheAsync(product.Id);
            return product;
        }

        public async Task<Product?> UpdateAsync(int id, Product updated)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return null;
            product.Name = updated.Name;
            product.Price = updated.Price;
            product.Stock = updated.Stock;
            await _db.SaveChangesAsync();
            // await InvalidateCacheAsync(id);
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            // await InvalidateCacheAsync(id);
            return true;
        }

        private async Task InvalidateCacheAsync(int productId)
        {
            await _cache.RemoveAsync(AllProductsKey);
            await _cache.RemoveAsync(ProductKey(productId));
        }
    }
}