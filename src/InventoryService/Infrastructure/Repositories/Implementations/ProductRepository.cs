using Shared.Entities;
using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> SearchSortAndFilterAsync(
            string? searchTerm,
            string? sortBy,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? category = null,
            bool? inStock = null)
        {
            var query = _context.Products
                .AsQueryable()
                .Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Origin.ToLower().Contains(term) ||
                    p.Category.ToString().ToLower().Contains(term));
            }

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (!string.IsNullOrWhiteSpace(category))
            {
                var categoryLower = category.ToLower();
                query = query.Where(p => p.Category.ToString().ToLower() == categoryLower);
            }

            if (inStock.HasValue)
            {
                query = inStock.Value
                    ? query.Where(p => p.Stock > 0)
                    : query.Where(p => p.Stock <= 0);
            }

            query = sortBy?.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "origin" => ascending ? query.OrderBy(p => p.Origin) : query.OrderByDescending(p => p.Origin),
                "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "stock" => ascending ? query.OrderBy(p => p.Stock) : query.OrderByDescending(p => p.Stock),
                "category" => ascending ? query.OrderBy(p => p.Category) : query.OrderByDescending(p => p.Category),
                _ => query.OrderBy(p => p.Name)
            };

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Remove(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string name, string origin, Guid? excludeProductId = null)
        {
            return await _context.Products.AnyAsync(p =>
                p.Name.ToLower() == name.ToLower() &&
                p.Origin.ToLower() == origin.ToLower() &&
                (!excludeProductId.HasValue || p.ProductId != excludeProductId.Value));
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var product = await GetByIdAsync(id);
            if (product == null || !product.IsActive) return false;

            product.Deactivate();
            return true;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var product = await GetByIdAsync(id);
            if (product == null || product.IsActive) return false;

            product.Activate();
            return true;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
{
    return await _context.Products
        .Where(p => p.IsActive)
        .AsNoTracking()
        .ToListAsync();
}

    }
}
