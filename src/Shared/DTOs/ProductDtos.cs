using System;
using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Shared.DTOs
{

    public class ProductCreateDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required, MaxLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public Category? Category { get; set; }
    }

        public class ProductUpdateDto
    {
        [MaxLength(200)]
        public string? Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }

        [MaxLength(100)]
        public string? Origin { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        public Category? Category { get; set; }
    }

    public class ProductResponseDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Required]
        public Category Category { get; set; }
    }
}
