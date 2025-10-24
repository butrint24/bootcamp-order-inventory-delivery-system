using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Shared.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(Guid userId, string role)
        {
            var claims = new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenKeyString = _config["AppSettings:TokenKey"]
                                ?? throw new Exception("TokenKey is not set");

            var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString));
            var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["AppSettings:AccessTokenExpiryMinutes"] ?? "15")
                ),
                SigningCredentials = credentials,
                Issuer = _config["AppSettings:Issuer"],
                Audience = _config["AppSettings:Audience"]
            };

            return new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityTokenHandler().CreateToken(descriptor)
            );
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenKeyString = _config["AppSettings:TokenKey"]
                                ?? throw new Exception("TokenKey is not set");

            var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString));

            try
            {
                return new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _config["AppSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["AppSettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = tokenKey,
                    ValidateIssuerSigningKey = true
                }, out _);
            }
            catch
            {
                return null;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            System.Security.Cryptography.RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public int GetRefreshTokenExpiryDays()
        {
            return int.Parse(_config["AppSettings:RefreshTokenExpiryDays"] ?? "7");
        }

        public int GetAccessTokenExpiryMinutes()
        {
            return int.Parse(_config["AppSettings:AccessTokenExpiryMinutes"] ?? "15");
        }

    }
}