using System;
using System.ComponentModel.DataAnnotations;
using Shared.Enums;


namespace Shared.DTOs
{

    public class DeliveryCreateDto
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public DateTime? Eta { get; set; }

        public DeliveryStatus? Status { get; set; }
    }

    public class DeliveryUpdateDto
    {
        public DeliveryStatus? Status { get; set; }

        public DateTime? Eta { get; set; } = null;
    }

    public class DeliveryResponseDto
    {
        [Required]
        public Guid DeliveryId { get; set; }

        [Required]
        public DeliveryStatus Status { get; set; }

        public DateTime? Eta { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public bool IsActive { get; set; }
    }
}
