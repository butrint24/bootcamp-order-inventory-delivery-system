using System.Runtime.CompilerServices;
using Shared.Entities;

namespace InventoryService.Infrastructure.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize);
        void Update(Product product);
        void Remove(Product product);
        Task<int> SaveChangesAsync();

        Task<bool> ExistsAsync(string name, string origin, Guid? excludeProductId = null);

        Task<bool> SoftDeleteAsync(Guid id);

        Task<bool> RestoreAsync(Guid id);

    }
}
