using OrderService.GrpcGenerated;
using Shared.DTOs.Order;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(OrderDto dto, Guid userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10);
        Task<OrderDto?> GetOrderByIdAsync(Guid id);
        Task<OrderDto?> UpdateOrderAsync(Guid id, OrderDto dto, Guid userId);
        Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(Guid id, OrderStatus status);
        Task<bool> DeleteOrderAsync(Guid id, Guid userId);
    }
}
