using Shared.DTOs.Order;
using Shared.Entities;
using Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Mapping
{
    public static class OrderMapping
    {
        public static Order ToEntity(OrderDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Order(dto.UserId, dto.Address?.Trim() ?? string.Empty)
            {
                Price = dto.Price,
                Status = ParseStatus(dto.Status)
            };
        }

        public static OrderDto ToDto(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                Address = order.Address,
                Price = order.Price,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt
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
    }
}
