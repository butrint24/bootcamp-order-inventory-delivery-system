using Shared.DTOs.Order;
using Shared.Entities;
using Shared.Enums;

namespace API.Mapping
{
    public static class OrderMapping
    {
        public static Order ToEntity(OrderDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var order = new Order(dto.UserId, dto.Address?.Trim() ?? string.Empty)
            {
                Price = dto.Price,
                Status = ParseStatus(dto.Status)
            };

            if (dto.Items != null && dto.Items.Any())
            {
                order.Items = dto.Items.Select(item => new OrderItem(order.OrderId, item.ProductId, item.Quantity)).ToList();
            }

            return order;
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
                CreatedAt = order.CreatedAt,
                Items = order.Items?.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList() ?? new List<OrderItemDto>()
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

            // Update items if provided
            if (dto.Items != null)
            {
                order.Items = dto.Items.Select(item => new OrderItem(order.OrderId, item.ProductId, item.Quantity)).ToList();
            }
        }

        private static OrderStatus ParseStatus(string? statusString)
        {
            if (Enum.TryParse<OrderStatus>(statusString ?? string.Empty, true, out var status))
                return status;

            return OrderStatus.PENDING;
        }
    }
}
