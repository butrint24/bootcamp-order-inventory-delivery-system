using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using OrderService.GrpcGenerated;
using Shared.DTOs;
using Shared.Entities;

namespace InventoryService.Application.Clients
{
    public class OrderGrpcClient
    {
        private readonly OrderService.GrpcGenerated.OrderService.OrderServiceClient _client;

        public OrderGrpcClient(OrderService.GrpcGenerated.OrderService.OrderServiceClient client)
        {
            _client = client;
        }

        public async Task<bool> IsOrderPersistedAsync(Guid orderId)
        {
            var request = new PersistenceCheck
            {
                OrderId = orderId.ToString()
            };

            var response = await _client.CheckOrderPersistenceAsync(request);
            return response.Persisted;
        }

        public async Task<List<OrderItemDto>> GetOrderItemsAsync(Guid orderId)
        {
            var request = new GetOrderItemsRequest
            {
                OrderId = orderId.ToString()
            };

            var response = await _client.GetOrderItemsAsync(request);
            return response.Items.Select(item => new OrderItemDto
            {
                OrderItemId = Guid.Parse(item.OrderItemId),
                OrderId = Guid.Parse(item.OrderId),
                ProductId = Guid.Parse(item.ProductId),
                Quantity = item.Quantity,
                CreatedAt = DateTime.Parse(item.CreatedAt)
            }).ToList();
        }
    }
}
