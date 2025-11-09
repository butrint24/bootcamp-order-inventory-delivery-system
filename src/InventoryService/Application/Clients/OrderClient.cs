using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using OrderService.GrpcGenerated;
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
    }
}
