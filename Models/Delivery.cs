namespace MyShopApi.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!; public string Address { get; set; } = null!;
        public DateTime DeliveryDate { get; set; }
    }
}
