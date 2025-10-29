using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Entities;
using Shared.DTOs;
using Shared.Enums;
using UserService.GrpcGenerated;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateAsync(UserCreateDto dto);
        Task<UserResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<UserResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<UserResponseDto?> UpdateAsync(Guid id, UserUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);
        Task<bool> ValidateUserAsync(Guid userId, RoleType? requiredRole);
    }
}
