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

        // Unified listing with search, sort, pagination
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10
        );

        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);
    }
}
