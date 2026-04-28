// Services/DeliveryService.cs
using Microsoft.EntityFrameworkCore;
using MyShopApi.Data;
using MyShopApi.Models;
using MyShopApi.Models.DTO;

namespace MyShopApi.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly ShopDbContext _db;

        public DeliveryService(ShopDbContext db)
        {
            _db = db;
        }

        public async Task<DeliveryResponseDto> CreateDeliveryAsync(DeliveryCreateDto dto)
        {
            var order = await _db.Orders.FindAsync(dto.OrderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            if (await _db.Deliveries.AnyAsync(d => d.OrderId == dto.OrderId))
                throw new InvalidOperationException("Delivery already assigned to this order");

            var delivery = new Delivery
            {
                OrderId = dto.OrderId,
                Address = dto.Address,
                DeliveryDate = dto.DeliveryDate ?? DateTime.UtcNow.AddDays(3)
            };
            await _db.Deliveries.AddAsync(delivery);
            await _db.SaveChangesAsync();

            return new DeliveryResponseDto
            {
                Id = delivery.Id,
                Address = delivery.Address,
                DeliveryDate = delivery.DeliveryDate
            };
        }

        public async Task<DeliveryResponseDto?> GetDeliveryByOrderIdAsync(int orderId)
        {
            var delivery = await _db.Deliveries
                .FirstOrDefaultAsync(d => d.OrderId == orderId);
            if (delivery == null) return null;
            return new DeliveryResponseDto
            {
                Id = delivery.Id,
                Address = delivery.Address,
                DeliveryDate = delivery.DeliveryDate
            };
        }

        public async Task<bool> UpdateDeliveryAsync(int deliveryId, string newAddress, DateTime newDate)
        {
            var delivery = await _db.Deliveries.FindAsync(deliveryId);
            if (delivery == null) return false;
            delivery.Address = newAddress;
            delivery.DeliveryDate = newDate;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDeliveryAsync(int deliveryId)
        {
            var delivery = await _db.Deliveries.FindAsync(deliveryId);
            if (delivery == null) return false;
            _db.Deliveries.Remove(delivery);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}