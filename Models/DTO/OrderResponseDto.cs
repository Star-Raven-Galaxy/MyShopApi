// Models/DTO/OrderResponseDto.cs
namespace MyShopApi.Models.DTO
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
        public DeliveryResponseDto? Delivery { get; set; }
    }

    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }

    public class DeliveryResponseDto
    {
        public int Id { get; set; }
        public string Address { get; set; } = "";
        public DateTime DeliveryDate { get; set; }
    }
}