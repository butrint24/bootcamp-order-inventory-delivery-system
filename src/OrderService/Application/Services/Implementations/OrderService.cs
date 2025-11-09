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
using Microsoft.Extensions.Logging;

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
                throw new UnauthorizedAccessException("User is not authorized to create an order.");

            dto.UserId = userId;
            var order = OrderMapping.ToEntity(dto);

            if (!Enum.IsDefined(typeof(OrderStatus), order.Status))
                order.Status = OrderStatus.PENDING;

            await _repo.AddAsync(order);
            await _repo.SaveChangesAsync();

            await _deliveryClient.CreateDeliveryAsync(order.OrderId, order.UserId);

            return OrderMapping.ToDto(order);
        }

        public async Task<OrderDto?> UpdateOrderAsync(Guid id, OrderDto dto, Guid userId)
        {
            var userValidation = await _userClient.ValidateUserAsync(userId, RoleType.Admin.ToString());
            if (!userValidation.Validated)
                throw new UnauthorizedAccessException("User is not authorized to update this order.");

            var order = await _repo.GetByIdAsync(id);
            if (order == null) return null;

            OrderMapping.UpdateEntity(order, dto);

            _repo.Update(order);
            await _repo.SaveChangesAsync();

            return OrderMapping.ToDto(order);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _repo.GetByIdAsync(id);
            return order == null ? null : OrderMapping.ToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10)
        {
            var orders = await _repo.GetAllAsync(pageNumber, pageSize);
            return orders.Select(OrderMapping.ToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersForUserAsync(Guid userId)
        {
            var orders = await _repo.GetByUserIdAsync(userId);
            return orders.Select(OrderMapping.ToDto);
        }

        public async Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(Guid id, OrderStatus status)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
                throw new ArgumentException("Invalid order status.");

            var order = await _repo.GetByIdAsync(id);
            if (order == null)
                throw new ArgumentException("Order not found.");

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

            if(shoppingCartDto.ItemsAndQuantities == null || !shoppingCartDto.ItemsAndQuantities.Any())
            {
                _logger.LogWarning("Shopping cart is empty for UserId: {UserId}", userId);
                throw new ArgumentException("Shopping cart cannot be empty.");
            }

            var userValidation = await _userClient.ValidateUserAsync(userId, "");

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

            foreach (var (productId, quantity) in shoppingCartDto.ItemsAndQuantities)
            {
                var orderItem = CreateOrderItemEntity(order.OrderId, productId, quantity);
                order.Items.Add(orderItem);
            }

            var productIdsAndQuantities = shoppingCartDto.ItemsAndQuantities
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var inventoryResponse = await _productClient.BuyProductsAsync(productIdsAndQuantities, order.OrderId);

            if (!inventoryResponse.Success)
            {
                _logger.LogWarning("Stock reservation failed for UserId: {UserId}, OrderId: {OrderId}", userId, order.OrderId);
                return null;
            }

            order.Price = inventoryResponse.GrpcProducts.Sum(p => (decimal)p.Price * productIdsAndQuantities[Guid.Parse(p.ProductId)]);

            var userInfo = await _userClient.GetUserInfoAsync(userId);
            order.Address = userInfo.Address;

            await _repo.AddAsync(order);

            await _repo.SaveChangesAsync();

            await _deliveryClient.CreateDeliveryAsync(order.OrderId, order.UserId);

            var orderDto = OrderMapping.ToDto(order);

            return orderDto;
        }

        public async Task<bool> IsOrderPersisted(Guid id)
        {
            var order = await _repo.GetByIdAsync(id);
            return order != null;
        }

        public async Task<bool> CancelOrderAsync(Guid id, Guid userId)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return false;

            if (order.UserId != userId)
            {
                throw new UnauthorizedAccessException("User is not authorized to cancel this order.");
            }

            if (order.Status != OrderStatus.PENDING && order.Status != OrderStatus.PROCESSING)
            {
                throw new InvalidOperationException("Order cannot be canceled.");
            }

            order.Status = OrderStatus.CANCELLED;
            _repo.Update(order);
            await _repo.SaveChangesAsync();

            Dictionary<Guid, int> productsToRestock = new Dictionary<Guid, int>();
            foreach (var item in order.Items)
            {
                productsToRestock[item.ProductId] = item.Quantity;
            }

            var response = await _productClient.RestockProductsAsync(productsToRestock, order.OrderId);
            if (!response.Success)
                _logger.LogWarning("Failed to restock products for canceled OrderId: {OrderId}", order.OrderId);

            else
            {
                await _deliveryClient.CancelDeliveryAsync(order.OrderId);
            }
            return true;
        }

        public async Task<bool> IsOrderCanceledAsync(Guid id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null)
            {
                throw new ArgumentException("Order not found.");
            }

            return order.Status == OrderStatus.CANCELLED;
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

        public async Task<ReturnOrderResponse> ReturnOrderAsync(Guid orderId, Guid userId)
        {
            _logger.LogInformation("Attempting to return order {OrderId} for user {UserId}", orderId, userId);

            var order = await _repo.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Return failed: Order {OrderId} not found.", orderId);
                return new ReturnOrderResponse { Success = false, Message = "Order not found." };
            }

            var userValidation = await _userClient.ValidateUserAsync(userId, RoleType.Admin.ToString());
            if (order.UserId != userId && !userValidation.Validated)
            {
                _logger.LogWarning("Authorization failed: User {UserId} cannot return order {OrderId}.", userId, orderId);
                return new ReturnOrderResponse { Success = false, Message = "Not authorized to return this order." };
            }

            if (order.Status == OrderStatus.RETURNED)
            {
                _logger.LogWarning("Return failed: Order {OrderId} has already been returned.", orderId);
                return new ReturnOrderResponse { Success = false, Message = "This order has already been returned." };
            }

            if (order.Status != OrderStatus.DELIVERED && order.Status != OrderStatus.COMPLETED)
            {
                _logger.LogWarning("Return failed: Order {OrderId} is in status {Status}, which is not returnable.", orderId, order.Status);
                return new ReturnOrderResponse { Success = false, Message = "Order cannot be returned at this stage." };
            }

            var originalStatus = order.Status;

            try
            {
                order.Status = OrderStatus.RETURNED;
                _repo.Update(order);
                await _repo.SaveChangesAsync();
                _logger.LogInformation("Order {OrderId} status updated to RETURNED in the database.", orderId);

                var productIdsAndQuantities = order.Items.ToDictionary(i => i.ProductId, i => i.Quantity);
                await _productClient.RollbackProductsAsync(productIdsAndQuantities);
                _logger.LogInformation("Inventory rollback successful for order {OrderId}.", orderId);

                return new ReturnOrderResponse
                {
                    Success = true,
                    Message = "Order returned successfully.",
                    OrderId = orderId.ToString(),
                    UpdatedStatus = OrderStatus.RETURNED.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process return for order {OrderId}. Reverting order status.", orderId);

                order.Status = originalStatus;
                _repo.Update(order);
                await _repo.SaveChangesAsync();

                return new ReturnOrderResponse
                {
                    Success = false,
                    Message = "Failed to process return due to an external inventory service error. Please try again later."
                };
            }
        }
    }
}
