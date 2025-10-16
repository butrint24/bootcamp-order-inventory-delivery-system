using System;
using System.Linq;
using Shared.Enums;

namespace Shared.Entities
{
    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();

        public decimal Price { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string Address { get; set; } = null!;

        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new();

        public Order() { }

        public Order(Guid userId, string address)
        {
            UserId = userId;
            Address = address;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(Guid productId, int qty, decimal unitPrice)
        {
            if (qty <= 0)
                throw new ArgumentOutOfRangeException(nameof(qty));

            Items.Add(new OrderItem(OrderId, productId, qty, unitPrice));
            Recalculate();
        }

        public void RemoveItem(Guid orderItemId)
        {
            var item = Items.FirstOrDefault(i => i.OrderItemId == orderItemId);
            if (item is null) return;
            Items.Remove(item);
            Recalculate();
        }

        public void MarkProcessing() => Status = OrderStatus.Processing;
        public void MarkCompleted()  => Status = OrderStatus.Completed;
        public void MarkCancelled()  => Status = OrderStatus.Cancelled;
        public void Confirm()        => Status = OrderStatus.Confirmed;

        private void Recalculate()
            => Price = Items.Sum(i => i.LineTotal);
    }
}
