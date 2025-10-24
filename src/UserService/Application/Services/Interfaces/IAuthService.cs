using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Entities;
using Shared.DTOs;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> SignupAsync(SignupRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<TokenResponseDto> RefreshTokenAsync(RefreshRequestDto dto);
    }
}
