using System;
using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Shared.DTOs
{
    public class UserCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Surname { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        [Required, Phone]
        public string Tel { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public RoleType Role { get; set; } = RoleType.User;
    }

    public class UserUpdateDto
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Surname { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        [Phone]
        public string Tel { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        public RoleType Role { get; set; }
    }

    public class UserResponseDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Surname { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        [Required]
        [Phone]
        public string Tel { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        [Required]
        public RoleType Role { get; set; }
    }

    public class SignupRequestDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Surname { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Phone]
        public string? Tel { get; set; }

        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }

        public RoleType Role { get; set; } = RoleType.User;
    }

    public class LoginRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public UserResponseDto User { get; set; } = new();
        public TokenResponseDto Tokens { get; set; } = new();
    }
}
