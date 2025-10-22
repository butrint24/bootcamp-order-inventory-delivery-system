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
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);
    }
}
