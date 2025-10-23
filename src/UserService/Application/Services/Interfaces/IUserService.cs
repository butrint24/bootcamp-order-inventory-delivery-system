using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Entities;
using Shared.DTOs;

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

        // Task<UserAuthResponseDto> SignupAsync(UserSignupDto dto);
        // Task<UserAuthResponseDto> LoginAsync(UserLoginDto dto);
        // Task<UserAuthResponseDto?> RefreshTokenAsync(string refreshToken);
    }
}
