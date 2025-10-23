using AutoMapper;
using Application.DTOs;
using Shared.Entities;
using System;

namespace DeliveryService.API.Mapping
{
    public class DeliveryProfile : Profile
    {
        public DeliveryProfile()
        {
            CreateMap<DeliveryCreateDto, Delivery>()
                .ForMember(d => d.DeliveryId, opt => opt.Ignore())
                .ForMember(d => d.Status, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.Ignore());

            CreateMap<DeliveryUpdateDto, Delivery>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.Ignore());

            CreateMap<Delivery, DeliveryResponseDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        }
    }
}
