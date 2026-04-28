// Services/IProductService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using MyShopApi.Models;

namespace MyShopApi.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);
    }
}