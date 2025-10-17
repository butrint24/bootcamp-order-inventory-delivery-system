using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using InventoryService.Infrastructure.Repositories.Interfaces;
using Shared.Entities;

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
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Product name is required.", nameof(dto.Name));
            if (dto.Price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(dto.Price));
            if (dto.Stock < 0)
                throw new ArgumentException("Stock cannot be negative.", nameof(dto.Stock));
            if (string.IsNullOrWhiteSpace(dto.Origin))
                throw new ArgumentException("Origin is required.", nameof(dto.Origin));

            var exists = await _repo.ExistsAsync(dto.Name, dto.Origin);
            if (exists)
                throw new InvalidOperationException("A product with the same name and origin already exists.");

            Product product = _mapper.Map<Product>(dto);
            product.IsActive = true; 
            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
        {
            var product = await _repo.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var products = await _repo.GetAllAsync(pageNumber, pageSize);
            return products?.Select(p => _mapper.Map<ProductResponseDto>(p)) ?? Enumerable.Empty<ProductResponseDto>();
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            _mapper.Map(dto, existing);

            _repo.Update(existing);
            await _repo.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var deleted = await _repo.SoftDeleteAsync(id);
            if (deleted) await _repo.SaveChangesAsync();
            return deleted;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var restored = await _repo.RestoreAsync(id);
            if (restored) await _repo.SaveChangesAsync();
            return restored;
        }
    }
}
