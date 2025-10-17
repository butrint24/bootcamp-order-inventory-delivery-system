using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> CreateAsync(Product product);
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid id);

        Task<bool> RestoreAsync(Guid id);
    }
}
