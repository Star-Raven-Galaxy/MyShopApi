// Services/CustomerService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyShopApi.Data;
using MyShopApi.Models;

namespace MyShopApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ShopDbContext _db;

        public CustomerService(ShopDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _db.Customers.ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _db.Customers.FindAsync(id);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> UpdateAsync(int id, Customer updated)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
                return null;

            customer.FullName = updated.FullName;
            customer.Email = updated.Email;
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
                return false;

            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}