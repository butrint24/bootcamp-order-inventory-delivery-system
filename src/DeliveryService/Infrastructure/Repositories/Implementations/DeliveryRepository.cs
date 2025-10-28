using DeliveryService.Infrastructure.Data;
using DeliveryService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace DeliveryService.Infrastructure.Repositories.Implementations
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly DeliveryDbContext _context;

        public DeliveryRepository(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Delivery delivery)
        {
            await _context.Deliveries.AddAsync(delivery);
        }

        public async Task<Delivery?> GetByIdAsync(Guid id)
        {
            return await _context.Deliveries
                .FirstOrDefaultAsync(d => d.DeliveryId == id);
        }

        public async Task<IEnumerable<Delivery>> SearchSortAndFilterAsync(
            string? searchTerm,
            string? sortBy,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10,
            DateTime? minEta = null,
            DateTime? maxEta = null,
            string? status = null,
            Guid? orderId = null,
            Guid? userId = null)
        {
            var query = _context.Deliveries
                .AsQueryable()
                .Where(d => d.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(d =>
                    d.Status.ToLower().Contains(term) ||
                    d.OrderId.ToString().ToLower().Contains(term) ||
                    d.UserId.ToString().ToLower().Contains(term));
            }

            if (minEta.HasValue)
                query = query.Where(d => d.Eta >= minEta.Value);

            if (maxEta.HasValue)
                query = query.Where(d => d.Eta <= maxEta.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                var statusLower = status.ToLower();
                query = query.Where(d => d.Status.ToLower() == statusLower);
            }

            if (orderId.HasValue)
                query = query.Where(d => d.OrderId == orderId.Value);

            if (userId.HasValue)
                query = query.Where(d => d.UserId == userId.Value);

            query = sortBy?.ToLower() switch
            {
                "status" => ascending ? query.OrderBy(d => d.Status) : query.OrderByDescending(d => d.Status),
                "eta" => ascending ? query.OrderBy(d => d.Eta) : query.OrderByDescending(d => d.Eta),
                "createdat" => ascending ? query.OrderBy(d => d.CreatedAt) : query.OrderByDescending(d => d.CreatedAt),
                "orderid" => ascending ? query.OrderBy(d => d.OrderId) : query.OrderByDescending(d => d.OrderId),
                "userid" => ascending ? query.OrderBy(d => d.UserId) : query.OrderByDescending(d => d.UserId),
                _ => query.OrderBy(d => d.CreatedAt)
            };

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public void Update(Delivery delivery)
        {
            _context.Deliveries.Update(delivery);
        }

        public void Remove(Delivery delivery)
        {
            _context.Deliveries.Remove(delivery);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var delivery = await GetByIdAsync(id);
            if (delivery == null || !delivery.IsActive)
                return false;

            delivery.IsActive = false;
            _context.Deliveries.Update(delivery);
            return true;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var delivery = await GetByIdAsync(id);
            if (delivery == null || delivery.IsActive)
                return false;

            delivery.IsActive = true;
            _context.Deliveries.Update(delivery);
            return true;
        }

    }
}
