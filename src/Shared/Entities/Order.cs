using System;
using System.Collections.Generic;
using Shared.Enums;

namespace Shared.Entities
{
    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.PENDING;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;


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
