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
        private readonly ProductClient _productClient;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository repo, DeliveryGrpcClient deliveryClient, UserGrpcClient userClient, ProductClient productClient, ILogger<OrderService> logger)
        {
            _repo = repo;
            _deliveryClient = deliveryClient;
            _userClient = userClient;
            _productClient = productClient;
            _logger = logger;
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
            _logger.LogInformation("Starting BuyCart for UserId: {UserId}", userId);

            var userValidation = await _userClient.ValidateUserAsync(userId, "");
            _logger.LogInformation("User validation result for UserId {UserId}: {Validated}", userId, userValidation.Validated);

            if (!userValidation.Validated)
            {
                _logger.LogWarning("User {UserId} is not authorized to create an order.", userId);
                throw new UnauthorizedAccessException("User is not authorized to create an order.");
            }

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.PENDING,
                Items = new List<OrderItem>()
            };
            _logger.LogInformation("Created new order entity with temporary Id: {OrderId}", order.OrderId);

            foreach (var (productId, quantity) in shoppingCartDto.ItemsAndQuantities)
            {
                var orderItem = CreateOrderItemEntity(order.OrderId, productId, quantity);
                order.Items.Add(orderItem);
                _logger.LogInformation("Added OrderItem: ProductId {ProductId}, Quantity {Quantity}", productId, quantity);
            }

            var productIdsAndQuantities = shoppingCartDto.ItemsAndQuantities
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            _logger.LogInformation("Requesting product reservation from InventoryService for OrderId: {OrderId}", order.OrderId);
            var inventoryResponse = await _productClient.GetProductsAsync(productIdsAndQuantities, order.OrderId);

            if (!inventoryResponse.Success)
            {
                _logger.LogError("Failed to reserve stock for OrderId: {OrderId}", order.OrderId);
                throw new InvalidOperationException("Failed to reserve stock for some products.");
            }

            order.Price = inventoryResponse.GrpcProducts.Sum(p => (decimal)p.Price * productIdsAndQuantities[Guid.Parse(p.ProductId)]);
            _logger.LogInformation("Calculated total order price for OrderId {OrderId}: {Price}", order.OrderId, order.Price);

            order.Address = "some address"; // get from user service
            _logger.LogInformation("Set order address for OrderId {OrderId}: {Address}", order.OrderId, order.Address);

            await _repo.AddAsync(order);
            _logger.LogInformation("Order entity added to repository for OrderId {OrderId}", order.OrderId);

            await _repo.SaveChangesAsync();
            _logger.LogInformation("Order saved to database for OrderId {OrderId}", order.OrderId);

            await _deliveryClient.CreateDeliveryAsync(order.OrderId, order.UserId);
            _logger.LogInformation("Delivery created for OrderId {OrderId}", order.OrderId);

            var orderDto = OrderMapping.ToDto(order);
            _logger.LogInformation("Returning OrderDto for OrderId {OrderId}", order.OrderId);

            return orderDto;
        }

        public async Task<bool> IsOrderPersisted(Guid id)
        {
            var order = await _repo.GetByIdAsync(id);
            return order != null;
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
