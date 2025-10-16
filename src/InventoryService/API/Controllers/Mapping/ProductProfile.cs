using AutoMapper;
using Application.DTOs;
using Shared.Entities;

namespace InventoryService.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductResponseDto>();
        }
    }
}
