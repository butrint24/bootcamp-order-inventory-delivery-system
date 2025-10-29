using System;
using System.ComponentModel.DataAnnotations;
using Shared.Enums; 

namespace Shared.DTOs
{
    public class DeliveryCreateDto
    {
        [Required]
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

        public DateTime? Eta { get; set; }
        public DeliveryStatus? Status { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class DeliveryUpdateDto
    {
        public DeliveryStatus? Status { get; set; }

        public DateTime? Eta { get; set; } = null;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class DeliveryResponseDto
    {
        public Guid DeliveryId { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime? Eta { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
