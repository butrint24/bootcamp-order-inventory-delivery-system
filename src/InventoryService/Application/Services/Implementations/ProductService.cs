using Shared.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using InventoryService.Infrastructure.Repositories.Interfaces;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            var exists = await _repo.ExistsAsync(dto.Name, dto.Origin);
            if (exists)
                throw new InvalidOperationException("A product with the same name and origin already exists.");

            var product = _mapper.Map<Product>(dto);
            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
        {
            var product = await _repo.GetByIdAsync(id);
            return product is null ? null : _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var products = await _repo.SearchAndSortAsync(searchTerm, sortBy, ascending, pageNumber, pageSize);
            return products?.Select(p => _mapper.Map<ProductResponseDto>(p)) ?? Enumerable.Empty<ProductResponseDto>();
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return null;

            if (await _repo.ExistsAsync(
                    string.IsNullOrWhiteSpace(dto.Name) ? existing.Name : dto.Name,
                    string.IsNullOrWhiteSpace(dto.Origin) ? existing.Origin : dto.Origin,
                    existing.ProductId))
            {
                throw new InvalidOperationException("Another product with the same name and origin already exists.");
            }

            existing.UpdateDetails(
                string.IsNullOrWhiteSpace(dto.Name) ? existing.Name : dto.Name,
                string.IsNullOrWhiteSpace(dto.Origin) ? existing.Origin : dto.Origin,
                dto.Category ?? existing.Category,
                dto.Price ?? existing.Price
            );

            if (dto.Stock.HasValue)
                existing.UpdateStock(dto.Stock.Value);

            _repo.Update(existing);
            await _repo.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var deleted = await _repo.SoftDeleteAsync(id);
            if (deleted)
                await _repo.SaveChangesAsync();
            return deleted;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var restored = await _repo.RestoreAsync(id);
            if (restored)
                await _repo.SaveChangesAsync();
            return restored;
        }
    }
}
