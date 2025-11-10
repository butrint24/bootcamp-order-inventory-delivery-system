using System;
using System.Threading.Tasks;
using UserService.Infrastructure.Repositories.Interfaces;
using Shared.Entities;
using Shared.Enums;
using Shared.Helpers;

public static class UserSeeder
{
    public static async Task SeedAdminsAsync(IUserRepository userRepository, JwtHelper jwtHelper)
    {
        var admins = new[]
        {
            new { Name="Admin1", Surname="User", Email="admin1@example.com", Tel="1234567890", Address="Admin Street 1" },
            new { Name="Admin2", Surname="User", Email="admin2@example.com", Tel="1234567891", Address="Admin Street 2" },
            new { Name="Admin3", Surname="User", Email="admin3@example.com", Tel="1234567892", Address="Admin Street 3" },
        };

        foreach (var a in admins)
        {
            var exists = await userRepository.GetByEmailAsync(a.Email);
            if (exists != null) continue;

            var user = new User(
                a.Name,
                a.Surname,
                null,
                a.Tel,
                a.Email,
                a.Address,
                RoleType.Admin
            );

            await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

            var refreshToken = jwtHelper.GenerateRefreshToken();
            var refreshTokenExpiryDays = jwtHelper.GetRefreshTokenExpiryDays();

            var auth = new UserAuth(
                user.UserId,
                passwordHash,
                refreshToken,
                DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await userRepository.AddAuthAsync(auth);
            await userRepository.SaveChangesAsync();
        }

        Console.WriteLine("Admin users seeded successfully.");
    }
}