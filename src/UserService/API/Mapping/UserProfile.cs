using AutoMapper;
using Shared.DTOs;
using Shared.Entities;
using Shared.Enums;
using System;

namespace UserService.API.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreateDto, User>()
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.Ignore());

            CreateMap<UserUpdateDto, User>()
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.Ignore());

            CreateMap<User, UserResponseDto>();
        }
    }
}
