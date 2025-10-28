using System;

namespace Shared.Entities
{
    public class Delivery
    {
        public Guid DeliveryId { get; private set; } = Guid.NewGuid();

        
        public string Status { get; private set; } = "PENDING";

        private DateTime? _eta;
        public DateTime? Eta
        {
            get => _eta;
            set => _eta = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }

        private DateTime _createdAt = DateTime.UtcNow;
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public bool IsActive { get; set; } = true;
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }

        private Delivery() { }

        public Delivery(Guid orderId, Guid userId, DateTime? eta = null)
        {
            OrderId = orderId;
            UserId = userId;
            Eta = eta;
            Status = "PENDING";
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void MarkProcessing()
        {
            if (Status != "PENDING")
                throw new InvalidOperationException("Delivery can only be processed from pending state.");
            Status = "PROCESSING";
        }
        public void MarkOnRoute()
        {
            if (Status != "PROCESSING")
                throw new InvalidOperationException("Delivery must be in processing state before being marked on route.");
            Status = "ON_ROUTE";
        }
        public void MarkDelivered()
        {
            if (Status != "ON_ROUTE")
                throw new InvalidOperationException("Delivery must be on route before being delivered.");
            Status = "DELIVERED";
        }

        public void UpdateEta(DateTime newEta)
        {
            if (newEta < DateTime.UtcNow)
                throw new InvalidOperationException("ETA cannot be in the past.");

            Eta = DateTime.SpecifyKind(newEta, DateTimeKind.Utc);
        }
    }
}
