using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Order
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }

        // [Required(ErrorMessage = "UserId is required.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, ErrorMessage = "Address cannot be longer than 250 characters.")]
        public string Address { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "OTHER";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
