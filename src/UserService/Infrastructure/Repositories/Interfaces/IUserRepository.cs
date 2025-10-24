using Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace UserService.Infrastructure.Repositories.Interfaces
{

    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllAsync(int pageNumber, int pageSize);
        void Update(User user);
        void Remove(User user);
        Task<int> SaveChangesAsync();
        Task<bool> ExistsAsync(string tel, string email, Guid? excludeUserId = null);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);

        Task AddAuthAsync(UserAuth auth);
        Task<UserAuth?> GetAuthByUserIdAsync(Guid userId);
        Task<UserAuth?> GetAuthByRefreshTokenAsync(string refreshToken);
        void UpdateAuth(UserAuth auth);
        Task<User?> GetByEmailAsync(string email);

    }
}   