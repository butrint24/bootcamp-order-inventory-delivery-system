using Application.Services.Interfaces;
using AutoMapper;
using InventoryService.Infrastructure.Repositories.Interfaces;
using Shared.DTOs;
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

        // Create me DTO + optional imageUrl (vendoset nga Controller)
        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            var exists = await _repo.ExistsAsync(dto.Name, dto.Origin);
            if (exists) throw new InvalidOperationException("A product with the same name and origin already exists.");

            var product = _mapper.Map<Product>(dto);
            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        // Overload: pranon imageUrl
        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto, string? imageUrl)
        {
            var exists = await _repo.ExistsAsync(dto.Name, dto.Origin);
            if (exists) throw new InvalidOperationException("A product with the same name and origin already exists.");

            var product = _mapper.Map<Product>(dto);
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                // nëse ke metodë në entitet, përdore; ndryshe sete direkt
                product.ImageUrl = imageUrl;
            }

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
            int pageSize = 10,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? category = null,
            bool? inStock = null)
        {
            var products = await _repo.SearchSortAndFilterAsync(
                searchTerm, sortBy, ascending, pageNumber, pageSize, minPrice, maxPrice, category, inStock
            );

            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null) return null;

            _mapper.Map(dto, product);
            await _repo.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var ok = await _repo.SoftDeleteAsync(id);
            if (!ok) return false;
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var ok = await _repo.RestoreAsync(id);
            if (!ok) return false;
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
