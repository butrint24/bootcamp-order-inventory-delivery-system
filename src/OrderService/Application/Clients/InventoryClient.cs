using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using InventoryService.GrpcGenerated;

namespace OrderService.Application.Clients
{
    public class InventoryGrpcClient
    {
        private readonly InventoryService.GrpcGenerated.InventoryService.InventoryServiceClient _client;

        public InventoryGrpcClient(InventoryService.GrpcGenerated.InventoryService.InventoryServiceClient client)
        {
            _client = client;
        }

        public async Task<DecreaseStockResponse> DecreaseStockAsync(Guid productId, int quantity)
        {
            var request = new DecreaseStockRequest
            {
                ProductId = productId.ToString(),
                Quantity = quantity
            };

            return await _client.DecreaseStockAsync(request);
        }
    }
}
