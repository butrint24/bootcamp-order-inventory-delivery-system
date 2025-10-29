using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Data;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Infrastructure.Repositories.Interfaces;

namespace OrderService.Infrastructure.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            // First, try to parse the incoming string into a Guid.
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                // If the string is not a valid Guid, return an empty list.
                return Enumerable.Empty<Order>();
            }

            // Now, use the converted 'userGuid' in the query.
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userGuid) // This now correctly compares a Guid to a Guid
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Orders
                // .Include(o => o.Items)
                .OrderBy(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }



        public void Remove(Order order)
        {
            _context.Orders.Remove(order);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}