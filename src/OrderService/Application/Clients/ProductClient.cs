using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryService.GrpcGenerated;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Clients
{
    public class ProductClient
    {
        private readonly ProductService.ProductServiceClient _client;
        private readonly ILogger<ProductClient> _logger;

        public ProductClient(ProductService.ProductServiceClient client, ILogger<ProductClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<GetProductsResponse> BuyProductsAsync(Dictionary<Guid, int> productIdsAndQuantities, Guid orderId)
        {
            var request = new BuyProductsMessage
            {
                OrderId = orderId.ToString()
            };

            request.IdsAndQuantities.Clear();

            foreach (var kvp in productIdsAndQuantities)
            {
                request.IdsAndQuantities.Add(kvp.Key.ToString(), kvp.Value);
            }

            try
            {
                var response = await _client.BuyProductsAsync(request);
                _logger.LogInformation("Received response for OrderId: {OrderId}, Success: {Success}, ProductsCount: {Count}",
                    orderId, response.Success, response.GrpcProducts.Count);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling BuyProducts for OrderId: {OrderId}", orderId);
                throw;
            }
        }

        public async Task RollbackProductsAsync(Dictionary<Guid, int> productIdsAndQuantities)
        {
            var request = new RollbackProductsMessage(); // FIX: Use correct message type

            foreach (var kvp in productIdsAndQuantities)
            {
                request.IdsAndQuantities.Add(kvp.Key.ToString(), kvp.Value);
            }

            try
            {
                var response = await _client.RollbackProductsAsync(request);
                if (!response.Success)
                    throw new Exception($"Failed to rollback products: {response.Message}");

                _logger.LogInformation("Rollback successful for {Count} products", productIdsAndQuantities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during product rollback");
                throw;
            }
        }
    }
}
