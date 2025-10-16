namespace Shared.DTOs.Order
{
    public class CreateOrderDto
    {
        public decimal Price { get; set; }
        public string Address { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
