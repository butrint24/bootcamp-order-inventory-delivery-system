using System;
using System.ComponentModel.DataAnnotations;


namespace Application.DTOs
{

    public class DeliveryCreateDto
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public DateTime? Eta { get; set; }
    }

    public class DeliveryUpdateDto
    {
        public DeliveryStatus? Status { get; set; }

        public DateTime? Eta { get; set; } = string.Empty();
    }

    public class DeliveryResponseDto
    {
        public Guid DeliveryId { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime? Eta { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
