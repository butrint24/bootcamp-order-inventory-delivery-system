using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class OrderItemDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
