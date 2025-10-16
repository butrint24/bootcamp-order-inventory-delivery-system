using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;

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
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .OrderBy(p => p.Name) 
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

        public async Task<bool> ExistsAsync(string name, string origin)
        {
            return await _context.Products
                .AnyAsync(p => p.Name.ToLower() == name.ToLower() && p.Origin.ToLower() == origin.ToLower());
        }

    }
}
