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

        public async Task<IEnumerable<Delivery>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Deliveries
                .Where(d => d.IsActive)
                .OrderBy(d => d.CreatedAt)
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
