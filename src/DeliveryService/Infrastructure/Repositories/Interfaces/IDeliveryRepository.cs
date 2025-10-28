using System.Runtime.CompilerServices;
using Shared.Entities;

namespace DeliveryService.Infrastructure.Repositories.Interfaces
{
    public interface IDeliveryRepository
    {
        Task AddAsync(Delivery delivery);
        Task<Delivery?> GetByIdAsync(Guid id);
        Task<IEnumerable<Delivery>> SearchSortAndFilterAsync(
            string? searchTerm,
            string? sortBy,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10,
            DateTime? minEta = null,
            DateTime? maxEta = null,
            string? status = null,
            Guid? orderId = null,
            Guid? userId = null);
        void Update(Delivery delivery);
        void Remove(Delivery delivery);
        Task<int> SaveChangesAsync();

        Task<bool> SoftDeleteAsync(Guid id);

        Task<bool> RestoreAsync(Guid id);
    }
}
