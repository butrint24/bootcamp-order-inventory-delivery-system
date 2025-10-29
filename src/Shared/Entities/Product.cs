using System;
using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Shared.Entities
{
    public class Product
    {
        [Required]
        public Guid ProductId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        [StringLength(100)]
        public string Origin { get; set; } = null!;

        [Required]
        public Category Category { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        private Product() { }

        public Product(
            string name,
            int stock,
            string origin,
            Category category,
            decimal price
        )
        {
            Name = name;
            Stock = stock;
            Origin = origin;
            Category = category;
            Price = price;
            IsActive = true;
        }

        public void UpdateStock(int newStock)
        {
            if (newStock < 0)
                throw new InvalidOperationException("Stock cannot be negative.");

            Stock = newStock;
        }


        public void UpdateDetails(string name, string origin, Category category, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.");

            if (price < 0)
                throw new InvalidOperationException("Price cannot be negative.");

            Name = name;
            Origin = origin;
            Category = category;
            Price = price;
        }


        public bool CanReserve(int qty) => qty > 0 && Stock >= qty;

        public void Reserve(int qty)
        {
            if (!CanReserve(qty))
                throw new InvalidOperationException("Not enough stock to reserve.");

            Stock -= qty;
        }

        public void Release(int qty)
        {
            if (qty <= 0)
                throw new InvalidOperationException("Quantity must be positive.");

            Stock += qty;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("Product is already inactive.");
            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Product is already active.");
            IsActive = true;
        }
         
    }
    
}
