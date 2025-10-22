using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);      
        Task<IEnumerable<Order>> GetAllAsync(int pageNumber, int pageSize);
        void Update(Order order);                       
        void Remove(Order order);
        Task<int> SaveChangesAsync();                     
    }
}