using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.Entities
{
    public class OrderItem
    {
        [Required]
        public Guid OrderItemId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
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
