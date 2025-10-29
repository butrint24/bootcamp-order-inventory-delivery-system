using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using OrderService.GrpcGenerated;

namespace DeliveryService.Application.Clients
{
    public class OrderGrpcClient
    {
        private readonly OrderService.GrpcGenerated.OrderService.OrderServiceClient _client;

        public OrderGrpcClient(OrderService.GrpcGenerated.OrderService.OrderServiceClient client)
        {
            _client = client;
        }

        public async Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(Guid orderId, string status)
        {
            var request = new UpdateOrderStatusRequest
            {
                OrderId = orderId.ToString(),
                Status = status
            };

            return await _client.UpdateOrderStatusAsync(request);
        }
    }
}
