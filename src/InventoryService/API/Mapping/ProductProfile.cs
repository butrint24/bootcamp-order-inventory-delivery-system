using AutoMapper;
using Application.DTOs;
using Shared.Entities;
using System;

namespace InventoryService.API.Controllers.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDto, Product>()
                .ForMember(d => d.ProductId, opt => opt.Ignore())
                .ForMember(d => d.IsActive,  opt => opt.Ignore())
                .ForMember(d => d.Category,  opt => opt.MapFrom(src => ParseCategory(src.Category)));

            CreateMap<ProductUpdateDto, Product>()
                .ForMember(d => d.IsActive,  opt => opt.Ignore())
                .ForMember(d => d.Category,  opt => opt.MapFrom(src => ParseCategory(src.Category)));

            CreateMap<Product, ProductResponseDto>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()));
        }

        private static Shared.Enums.Category ParseCategory(string? category)
        {
            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<Shared.Enums.Category>(category, true, out var parsed))
            {
                return parsed;
            }

            return default; 
        }
    }
}
