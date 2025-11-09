using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using DeliveryService.GrpcGenerated;


namespace OrderService.Application.Clients
{
    public class DeliveryGrpcClient
    {
        private readonly DeliveryService.GrpcGenerated.DeliveryService.DeliveryServiceClient _client;

        public DeliveryGrpcClient(DeliveryService.GrpcGenerated.DeliveryService.DeliveryServiceClient client)
        {
            _client = client;
        }

        public async Task<CreateDeliveryResponse> CreateDeliveryAsync(Guid orderId, Guid userId)
        {
            var request = new CreateDeliveryMessage
            {
                OrderId = orderId.ToString(),
                UserId = userId.ToString()
            };

            return await _client.createDeliveryAsync(request);
        }
    }
}
