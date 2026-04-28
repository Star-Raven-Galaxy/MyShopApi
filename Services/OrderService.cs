using Microsoft.EntityFrameworkCore;
using MyShopApi.Data;
using MyShopApi.Models;
using MyShopApi.Models.DTO;

namespace MyShopApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShopDbContext _db;
        private readonly ICacheService _cache;
        private const string CustomerOrdersKeyPrefix = "customer_orders_";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public OrderService(ShopDbContext db, ICacheService cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto dto)
        {
            var customer = await _db.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            decimal total = 0;
            foreach (var item in dto.Items)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product == null) continue;
                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Not enough stock for product {product.Name}");

                product.Stock -= item.Quantity;
                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
                total += item.Quantity * product.Price;
            }

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            await _cache.RemoveAsync(CustomerOrdersKeyPrefix + dto.CustomerId);
            return await MapToResponse(order, customer.FullName, total);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return null;

            var total = order.Items.Sum(i => i.Quantity * i.UnitPrice);
            var delivery = await _db.Deliveries.FirstOrDefaultAsync(d => d.OrderId == order.Id);
            return await MapToResponse(order, order.Customer.FullName, total, delivery);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByCustomerAsync(int customerId)
        {
            string cacheKey = CustomerOrdersKeyPrefix + customerId;
            return await _cache.GetOrSetAsync(cacheKey,
                async () =>
                {
                    var orders = await _db.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.Items).ThenInclude(i => i.Product)
                        .Where(o => o.CustomerId == customerId)
                        .ToListAsync();

                    var result = new List<OrderResponseDto>();
                    foreach (var order in orders)
                    {
                        var total = order.Items.Sum(i => i.Quantity * i.UnitPrice);
                        var delivery = await _db.Deliveries.FirstOrDefaultAsync(d => d.OrderId == order.Id);
                        result.Add(await MapToResponse(order, order.Customer.FullName, total, delivery));
                    }
                    return result;
                }, CacheTtl);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .ToListAsync();

            var result = new List<OrderResponseDto>();
            foreach (var order in orders)
            {
                var total = order.Items.Sum(i => i.Quantity * i.UnitPrice);
                var delivery = await _db.Deliveries.FirstOrDefaultAsync(d => d.OrderId == order.Id);
                result.Add(await MapToResponse(order, order.Customer.FullName, total, delivery));
            }
            return result;
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return false;
            // Если добавите поле Status, раскомментируйте:
            // order.Status = status;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return false;

            var delivery = await _db.Deliveries.FirstOrDefaultAsync(d => d.OrderId == id);
            if (delivery != null)
                _db.Deliveries.Remove(delivery);

            _db.OrderItems.RemoveRange(order.Items);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            await _cache.RemoveAsync(CustomerOrdersKeyPrefix + order.CustomerId);
            return true;
        }

        private async Task<OrderResponseDto> MapToResponse(Order order, string customerName, decimal total, Delivery? delivery = null)
        {
            if (delivery == null)
                delivery = await _db.Deliveries.FirstOrDefaultAsync(d => d.OrderId == order.Id);

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerFullName = customerName,
                OrderDate = order.OrderDate,
                TotalAmount = total,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Unknown",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                Delivery = delivery != null ? new DeliveryResponseDto
                {
                    Id = delivery.Id,
                    Address = delivery.Address,
                    DeliveryDate = delivery.DeliveryDate
                } : null
            };
        }
    }
}