using Application.Services.Interfaces;
using InventoryService.Infrastructure.Repositories.Interfaces;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }
        
        public async Task<Product> CreateAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name is required.", nameof(product.Name));

            if (product.Price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(product.Price));

            if (product.Stock < 0)
                throw new ArgumentException("Stock cannot be negative.", nameof(product.Stock));

            if (string.IsNullOrWhiteSpace(product.Origin))
                throw new ArgumentException("Origin is required.", nameof(product.Origin));

            var exists = await _repo.ExistsAsync(product.Name, product.Origin);
            if (exists)
                throw new InvalidOperationException("A product with the same name and origin already exists.");

            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            var product = await _repo.GetByIdAsync(id);

            return product;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            var products = await _repo.GetAllAsync(pageNumber, pageSize);

            if (products == null)
                throw new InvalidOperationException();

            return products;
        }
    }
}
