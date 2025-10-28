using Shared.Entities;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<Product?> GetByIdAsync(Guid id);

        Task<IEnumerable<Product>> SearchSortAndFilterAsync(
            string? searchTerm,
            string? sortBy,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? category = null,
            bool? inStock = null
        );

          Task<IEnumerable<Product>> GetAllAsync();

        void Update(Product product);
        void Remove(Product product);
        Task<int> SaveChangesAsync();

        Task<bool> ExistsAsync(string name, string origin, Guid? excludeProductId = null);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);
    }
}
