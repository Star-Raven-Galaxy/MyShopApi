// Models/DTO/DeliveryCreateDto.cs
namespace MyShopApi.Models.DTO
{
    public class DeliveryCreateDto
    {
        public int OrderId { get; set; }
        public string Address { get; set; } = "";
        public DateTime? DeliveryDate { get; set; } // если null – ставим через 3 дня
    }
}