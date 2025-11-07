using InventoryService.GrpcGenerated;
using Shared.DTOs;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto);
        Task<ProductResponseDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto);
        Task<ProductResponseDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<ProductResponseDto>> GetAllAsync(
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? category = null,
            bool? inStock = null
        );

        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);
        Task<GrpcProduct> ReserveProductStock(Guid productId, int quantity);
        Task RollbackProductStockAsync(Guid productId, int quantity);
        Task<bool> RestockProductStockAsync(Guid productId, int quantity);
        Task SaveChangesAsync();
    }
}
