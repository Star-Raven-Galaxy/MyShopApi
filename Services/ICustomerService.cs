// Services/ICustomerService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using MyShopApi.Models;

namespace MyShopApi.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer?> UpdateAsync(int id, Customer customer);
        Task<bool> DeleteAsync(int id);
    }
}