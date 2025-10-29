using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Shared.Entities
{
    public class Order
    {
        [Required]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = null!;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.PENDING;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public List<OrderItem> Items { get; set; } = new();

        public Order() { }

        public Order(Guid userId, string address)
        {
            UserId = userId;
            Address = address;
        }
    }
}
