using AutoMapper;
using Shared.DTOs;
using Shared.Entities;
using Shared.Enums;
using System;

namespace InventoryService.API.Controllers.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDto, Product>()
                .ForMember(d => d.ProductId, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.Ignore());

            CreateMap<ProductUpdateDto, Product>()
                .ForMember(d => d.IsActive, opt => opt.Ignore());

            CreateMap<Product, ProductResponseDto>();
        }
    }
}
