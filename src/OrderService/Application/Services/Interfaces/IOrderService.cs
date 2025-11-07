using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.DTOs.Order;
using Shared.DTOs;
using Shared.Enums;
using OrderService.GrpcGenerated;

namespace Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(OrderDto dto, Guid userId);
        Task<OrderDto?> UpdateOrderAsync(Guid id, OrderDto dto, Guid userId);
        Task<OrderDto?> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10);
        Task<bool> DeleteOrderAsync(Guid id, Guid userId);
        Task<IEnumerable<OrderDto>> GetOrdersForUserAsync(Guid userId);
        Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(Guid id, OrderStatus status);
        Task<OrderDto?> BuyCartAsync(ShoppingCartDto shoppingCartDto, Guid userId);
        Task<bool> IsOrderPersisted(Guid id);
        Task<bool> CancelOrderAsync(Guid id, Guid userId);
        Task<bool> IsOrderCanceledAsync(Guid id);
    }
}
