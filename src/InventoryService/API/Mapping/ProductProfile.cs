// src/InventoryService/API/Mapping/ProductProfile.cs  (ose Controllers/Mapping nëse aty e ke)
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

        // Helper i sigurt për expression tree (pa out var në expression)
        private static Shared.Enums.Category ParseCategory(string? category)
        {
            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<Shared.Enums.Category>(category, true, out var parsed))
            {
                return parsed;
            }

            // Fallback – vendose çfarë do: default ose një vlerë specifike
            // return Shared.Enums.Category.ACCESSORIES;
            return default; // vlera e parë e enum-it tënd
        }
    }
}
