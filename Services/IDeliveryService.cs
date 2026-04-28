// Services/IDeliveryService.cs
using MyShopApi.Models.DTO;

namespace MyShopApi.Services
{
    public interface IDeliveryService
    {
        Task<DeliveryResponseDto> CreateDeliveryAsync(DeliveryCreateDto dto);
        Task<DeliveryResponseDto?> GetDeliveryByOrderIdAsync(int orderId);
        Task<bool> UpdateDeliveryAsync(int deliveryId, string newAddress, DateTime newDate);
        Task<bool> DeleteDeliveryAsync(int deliveryId);
    }
}