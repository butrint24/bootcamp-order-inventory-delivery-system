using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.DTOs.Order;
using Shared.DTOs;
using Shared.Entities;
using Shared.Enums;
using Application.Services.Interfaces;
using OrderService.Infrastructure.Repositories.Interfaces;
using API.Mapping;
using OrderService.GrpcGenerated;
using OrderService.Application.Clients;
using Microsoft.AspNetCore.Http.Features;

namespace Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly DeliveryGrpcClient _deliveryClient;
        private readonly UserGrpcClient _userClient;

        public OrderService(IOrderRepository repo, DeliveryGrpcClient deliveryClient, UserGrpcClient userClient)
        {
            _repo = repo;
            _deliveryClient = deliveryClient;
            _userClient = userClient;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto dto, Guid userId)
        {
            var userValidation = await _userClient.ValidateUserAsync(userId, "");
            if (!userValidation.Validated)
            {
                throw new UnauthorizedAccessException("User is not authorized to create an order.");
            }
            dto.UserId = userId;
            var order = OrderMapping.ToEntity(dto);

            if (!Enum.IsDefined(typeof(OrderStatus), order.Status))
            {
                order.Status = OrderStatus.PENDING; 
            }

            await _repo.AddAsync(order);
            await _repo.SaveChangesAsync();

            await _deliveryClient.CreateDeliveryAsync(order.OrderId, order.UserId);

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

        public async Task<OrderDto?> UpdateOrderAsync(Guid id, OrderDto dto, Guid userId)
        {
            var userValidation = await _userClient.ValidateUserAsync(userId, RoleType.Admin.ToString());
            if (!userValidation.Validated)
            {
                throw new UnauthorizedAccessException("User is not authorized to update this order.");
            }

            var order = await _repo.GetByIdAsync(id);
            if (order == null) return null;

            OrderMapping.UpdateEntity(order, dto);

            _repo.Update(order);
            await _repo.SaveChangesAsync();

            return OrderMapping.ToDto(order);
        }

        public async Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(Guid id, OrderStatus status)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                throw new ArgumentException("Invalid order status.");
            }
            var order = await _repo.GetByIdAsync(id);
            if (order == null)
            {
                throw new ArgumentException("Order not found.");
            }

            order.Status = status;
            _repo.Update(order);
            await _repo.SaveChangesAsync();

            return new UpdateOrderStatusResponse
            {
                OrderId = order.OrderId.ToString(),
                UpdatedStatus = status.ToString()
            };
        }

        public async Task<bool> DeleteOrderAsync(Guid id, Guid userId)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return false;
            var userValidation = await _userClient.ValidateUserAsync(userId, RoleType.Admin.ToString());
            if (userId != order.UserId || !userValidation.Validated)
            {
                throw new UnauthorizedAccessException("User is not authorized to delete this order.");
            }

            _repo.Remove(order);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<OrderDto?> BuyCartAsync(ShoppingCartDto shoppingCartDto, Guid userId)
        {
            var userValidation = await _userClient.ValidateUserAsync(userId, "");
            if (!userValidation.Validated)
                throw new UnauthorizedAccessException("User is not authorized to create an order.");
            
            Order order = new()
            {
                UserId = userId,
                Status = OrderStatus.PENDING,
                Items = new List<OrderItem>()
            };

            foreach (var (productId, quantity) in shoppingCartDto.ItemsAndQuantities)
            {
                var orderItem = CreateOrderItemEntity(order.OrderId, productId, quantity);
                order.Items.Add(orderItem);
            }

            order.Price = 0; //calculate price from products

            // Get address from user service

            await _repo.AddAsync(order);
            await _repo.SaveChangesAsync();

            await _deliveryClient.CreateDeliveryAsync(order.OrderId, order.UserId);

            return OrderMapping.ToDto(order);
        }
        private OrderItem CreateOrderItemEntity(Guid orderId, Guid productId, int quantity)
        {
            return new OrderItem
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity
            };
        }

    }
}
