using System;
using Shared.Enums;

namespace Shared.Entities
{
    public class Delivery
    {
        public Guid DeliveryId { get; private set; } = Guid.NewGuid();

        public DeliveryStatus Status { get; private set; } = DeliveryStatus.Pending;

        public DateTime? Eta { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        private Delivery() { }

        public Delivery( DateTime? eta = null)
        {
            Eta = eta;
            Status = DeliveryStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkProcessing()
        {
            if (Status != DeliveryStatus.Pending)
                throw new InvalidOperationException("Delivery can only be processed from pending state.");

            Status = DeliveryStatus.Processing;
        }

        public void MarkDelivered()
        {
            if (Status != DeliveryStatus.Processing)
                throw new InvalidOperationException("Delivery must be in processing state before being delivered.");

            Status = DeliveryStatus.Delivered;
        }

        public void UpdateEta(DateTime newEta)
        {
            if (newEta < DateTime.UtcNow)
                throw new InvalidOperationException("ETA cannot be in the past.");

            Eta = newEta;
        }
    }
}
