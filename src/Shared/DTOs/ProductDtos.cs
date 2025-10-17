using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    
    public class ProductCreateDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required, MaxLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }
    }

        public class ProductUpdateDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required, MaxLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }
    }

    public class ProductResponseDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string? Category { get; set; }
    }
}
