using System;

namespace Shared.Entities
{
    public class OrderItem
    {
        public Guid OrderItemId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, int quantity)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
