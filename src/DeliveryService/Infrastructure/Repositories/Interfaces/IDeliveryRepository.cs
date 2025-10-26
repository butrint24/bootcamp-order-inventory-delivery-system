using System.Runtime.CompilerServices;
using Shared.Entities;

namespace DeliveryService.Infrastructure.Repositories.Interfaces
{
    public interface IDeliveryRepository
    {
        Task AddAsync(Delivery delivery);
        Task<Delivery?> GetByIdAsync(Guid id);
        Task<IEnumerable<Delivery>> GetAllAsync(int pageNumber, int pageSize);
        void Update(Delivery delivery);
        void Remove(Delivery delivery);
        Task<int> SaveChangesAsync();

        Task<bool> SoftDeleteAsync(Guid id);

        Task<bool> RestoreAsync(Guid id);
    }
}
