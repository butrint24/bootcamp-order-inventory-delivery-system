using Shared.Entities;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string tel, string email, Guid? excludeUserId = null)
        {
            return await _context.Users.AnyAsync(u =>
                (u.Tel == tel || u.Email == email) &&
                (!excludeUserId.HasValue || u.UserId != excludeUserId.Value));
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null || !user.IsActive) return false;

            user.Deactivate();
            return true;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null || user.IsActive) return false;

            user.Activate();
            return true;
        }

        public async Task AddAuthAsync(UserAuth auth)
        {
            await _context.UserAuths.AddAsync(auth);
        }

        public async Task<UserAuth?> GetAuthByUserIdAsync(Guid userId)
        {
            return await _context.UserAuths
                                 .FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task<UserAuth?> GetAuthByRefreshTokenAsync(string refreshToken)
        {
            return await _context.UserAuths
                                 .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken);
        }

        public void UpdateAuth(UserAuth auth)
        {
            _context.UserAuths.Update(auth);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}
