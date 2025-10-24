using System;
using System.Threading.Tasks;
using UserService.Infrastructure.Repositories.Interfaces;
using Shared.Entities;
using Shared.Helpers;
using Shared.DTOs;
using Application.Services.Interfaces;
using BCrypt.Net;

namespace Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto> SignupAsync(SignupRequestDto dto)
        {
            if (await _userRepository.ExistsAsync(dto.Tel ?? "", dto.Email))
                throw new InvalidOperationException("User with the same email or phone already exists.");

            var user = new User(
                dto.Name,
                dto.Surname,
                dto.DateOfBirth,
                dto.Tel ?? "",
                dto.Email,
                dto.Address ?? "",
                dto.Role
            );

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var refreshToken = _jwtHelper.GenerateRefreshToken();
            var refreshTokenExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

            var auth = new UserAuth(
                user.UserId,
                passwordHash,
                refreshToken,
                DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _userRepository.AddAuthAsync(auth);
            await _userRepository.SaveChangesAsync();

            var accessToken = _jwtHelper.CreateToken(user.UserId, user.Role.ToString());

            return new AuthResponseDto
            {
                User = MapToUserResponseDto(user),
                Tokens = new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtHelper.GetAccessTokenExpiryMinutes())
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var auth = await _userRepository.GetAuthByUserIdAsync(user.UserId);
            if (auth == null || !BCrypt.Net.BCrypt.Verify(dto.Password, auth.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var accessToken = _jwtHelper.CreateToken(user.UserId, user.Role.ToString());
            var refreshToken = _jwtHelper.GenerateRefreshToken();
            var refreshTokenExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

            if (auth == null)
            {
                auth = new UserAuth(user.UserId, BCrypt.Net.BCrypt.HashPassword(dto.Password), refreshToken, DateTime.UtcNow.AddDays(refreshTokenExpiryDays));
                await _userRepository.AddAuthAsync(auth);
            }
            else
            {
                auth.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(refreshTokenExpiryDays));
                _userRepository.UpdateAuth(auth);
            }

            await _userRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                User = MapToUserResponseDto(user),
                Tokens = new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtHelper.GetAccessTokenExpiryMinutes())
                }
            };
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(RefreshRequestDto dto)
        {
            var auth = await _userRepository.GetAuthByRefreshTokenAsync(dto.RefreshToken);
            if (auth == null || auth.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = await _userRepository.GetByIdAsync(auth.UserId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var newAccessToken = _jwtHelper.CreateToken(user.UserId, user.Role.ToString());
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();
            var refreshTokenExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

            auth.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(refreshTokenExpiryDays));
            _userRepository.UpdateAuth(auth);

            await _userRepository.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtHelper.GetAccessTokenExpiryMinutes())
            };
        }

        private UserResponseDto MapToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Tel = user.Tel,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }
    }
}
