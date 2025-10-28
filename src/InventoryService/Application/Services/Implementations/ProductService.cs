<<<<<<< HEAD
using Application.Services.Interfaces;
using AutoMapper;
=======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Application.Services.Interfaces;
>>>>>>> 9c4d298 (Add image upload support for Product)
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

<<<<<<< HEAD
        // Create me DTO + optional imageUrl (vendoset nga Controller)
=======
>>>>>>> 9c4d298 (Add image upload support for Product)
        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            var exists = await _repo.ExistsAsync(dto.Name, dto.Origin);
            if (exists) throw new InvalidOperationException("A product with the same name and origin already exists.");

            var product = _mapper.Map<Product>(dto);
            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();
<<<<<<< HEAD

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
=======
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto, string? imageUrl = null)
        {
            var exists = await _repo.ExistsAsync(dto.Name, dto.Origin);
            if (exists)
                throw new InvalidOperationException("A product with the same name and origin already exists.");

            var product = _mapper.Map<Product>(dto);
            product.ImageUrl = imageUrl;
>>>>>>> 9c4d298 (Add image upload support for Product)

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
            string? searchTerm,
            string? sortBy,
            bool ascending,
            int pageNumber,
            int pageSize,
            decimal? minPrice,
            decimal? maxPrice,
            string? category,
            bool? inStock)
        {
            var products = await _repo.SearchSortAndFilterAsync(
<<<<<<< HEAD
                searchTerm, sortBy, ascending, pageNumber, pageSize, minPrice, maxPrice, category, inStock
            );
=======
     searchTerm,
     sortBy,
     ascending,
     pageNumber,
     pageSize,
     minPrice,
     maxPrice,
     category,
     inStock
 );
>>>>>>> 9c4d298 (Add image upload support for Product)

            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }


        public async Task<ProductResponseDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null) return null;

<<<<<<< HEAD
            _mapper.Map(dto, product);
=======
            var newName = string.IsNullOrWhiteSpace(dto.Name) ? existing.Name : dto.Name;
            var newOrigin = string.IsNullOrWhiteSpace(dto.Origin) ? existing.Origin : dto.Origin;

            if (await _repo.ExistsAsync(newName, newOrigin, existing.ProductId))
                throw new InvalidOperationException("Another product with the same name and origin already exists.");

            existing.UpdateDetails(
                newName,
                newOrigin,
                dto.Category ?? existing.Category,
                dto.Price ?? existing.Price
            );

            if (dto.Stock.HasValue)
                existing.UpdateStock(dto.Stock.Value);

            _repo.Update(existing);
>>>>>>> 9c4d298 (Add image upload support for Product)
            await _repo.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
<<<<<<< HEAD
            var ok = await _repo.SoftDeleteAsync(id);
            if (!ok) return false;
            await _repo.SaveChangesAsync();
            return true;
=======
            var deleted = await _repo.SoftDeleteAsync(id);
            if (deleted) await _repo.SaveChangesAsync();
            return deleted;
>>>>>>> 9c4d298 (Add image upload support for Product)
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
<<<<<<< HEAD
            var ok = await _repo.RestoreAsync(id);
            if (!ok) return false;
            await _repo.SaveChangesAsync();
            return true;
=======
            var restored = await _repo.RestoreAsync(id);
            if (restored) await _repo.SaveChangesAsync();
            return restored;
>>>>>>> 9c4d298 (Add image upload support for Product)
        }
    }
}
