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

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public Category? Category { get; set; }
    }

    public class ProductUpdateDto
    {
        public string? Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }

        [MaxLength(100)]
        public string? Origin { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        public Category? Category { get; set; }
        
         public string? ImageUrl { get; set; }
    }

    public class ProductResponseDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string Origin { get; set; } = string.Empty;
        public Category Category { get; set; }

        
        public string? ImageUrl { get; set; }
    }
}
