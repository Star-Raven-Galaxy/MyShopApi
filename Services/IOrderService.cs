// Services/IOrderService.cs
using MyShopApi.Models.DTO;

namespace MyShopApi.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto dto);
        Task<OrderResponseDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByCustomerAsync(int customerId);
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(); // для админа
        Task<bool> UpdateOrderStatusAsync(int id, string status); // например, статус: Pending, Shipped, Delivered
        Task<bool> DeleteOrderAsync(int id);
    }
}