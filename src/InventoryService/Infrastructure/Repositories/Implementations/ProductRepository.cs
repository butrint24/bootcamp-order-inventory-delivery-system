using Shared.Entities;
    using InventoryService.Infrastructure.Data;
    using InventoryService.Infrastructure.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Where(p => p.IsActive)
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
    }
}
