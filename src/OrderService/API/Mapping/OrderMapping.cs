using Shared.DTOs.Order;
using Shared.Entities;
using Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace API.Mapping
{
    public static class OrderMapping
    {
        public static Order ToEntity(OrderDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            List<OrderItem> items = new();
            foreach (var item in dto.Items)
            {
                items.Add(ToItemEntity(item));
            }

            return new Order(dto.UserId, dto.Address?.Trim() ?? string.Empty)
            {
                Price = dto.Price,
                Status = ParseStatus(dto.Status),
                Items = items
            };
        }

        public static OrderDto ToDto(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            List<OrderItemDto> items = new();
            foreach(var item in order.Items)
            {
                items.Add(ToItemDto(item));
            }

            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                Address = order.Address,
                Price = order.Price,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                Items = items
            };
        }

        public static void UpdateEntity(Order order, OrderDto dto)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            order.UserId = dto.UserId;
            order.Address = dto.Address?.Trim() ?? string.Empty;
            order.Price = dto.Price;
            order.Status = ParseStatus(dto.Status);
        }

        private static OrderStatus ParseStatus(string? statusString)
        {
            if (Enum.TryParse<OrderStatus>(statusString ?? string.Empty, true, out var status))
                return status;

            return OrderStatus.PENDING;
        }

        private static OrderItemDto ToItemDto(OrderItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return new OrderItemDto
            {
                OrderItemId = item.OrderItemId,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                CreatedAt = item.CreatedAt
            };
        }

        private static OrderItem ToItemEntity(OrderItemDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new OrderItem
            {
                OrderItemId = dto.OrderItemId,
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                CreatedAt = dto.CreatedAt
            };
        }
    }
}
