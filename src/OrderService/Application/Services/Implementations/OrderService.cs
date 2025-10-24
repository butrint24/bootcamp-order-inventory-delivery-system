using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.DTOs.Order;
using Shared.Entities;
using Shared.Enums;
using Application.Services.Interfaces;
using OrderService.Infrastructure.Repositories.Interfaces;
using API.Mapping;

namespace Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto dto)
        {
            var order = OrderMapping.ToEntity(dto);

            if (!Enum.IsDefined(typeof(OrderStatus), order.Status))
            {
                order.Status = OrderStatus.PENDING; 
            }

            await _repo.AddAsync(order);
            await _repo.SaveChangesAsync();

            return OrderMapping.ToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10)
        {
            var orders = await _repo.GetAllAsync(pageNumber, pageSize);
            return orders.Select(OrderMapping.ToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _repo.GetByIdAsync(id);
            return order == null ? null : OrderMapping.ToDto(order);
        }

        public async Task<OrderDto?> UpdateOrderAsync(Guid id, OrderDto dto)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return null;

            OrderMapping.UpdateEntity(order, dto);

            _repo.Update(order);
            await _repo.SaveChangesAsync();

            return OrderMapping.ToDto(order);
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return false;

            _repo.Remove(order);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
