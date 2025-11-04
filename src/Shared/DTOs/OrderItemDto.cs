using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class OrderItemDto
    {
        public Guid OrderItemId { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
