using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Shared.DTOs;
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

            GetProductsResponse response;
            try
            {
                response = await _client.BuyProductsAsync(request);
                _logger.LogInformation("Received response for OrderId: {OrderId}, Success: {Success}, ProductsCount: {Count}",
                    orderId, response.Success, response.GrpcProducts.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling GetProducts for OrderId: {OrderId}", orderId);
                throw;
            }

            return response;
        }

        public async Task<RestockProductsResponse> RestockProductsAsync(Dictionary<Guid, int> productIdsAndQuantities, Guid orderId)
        {
            var request = new RestockProductsMessage
            {
                OrderId = orderId.ToString()
            };

            request.IdsAndQuantities.Clear();

            foreach (var kvp in productIdsAndQuantities)
            {
                request.IdsAndQuantities.Add(kvp.Key.ToString(), kvp.Value);
            }

            RestockProductsResponse response;
            try
            {
                response = await _client.RestockProductsAsync(request);
                _logger.LogInformation("Received restock response for OrderId: {OrderId}, Success: {Success}",
                    orderId, response.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling RestockProducts for OrderId: {OrderId}", orderId);
                throw;
            }

            return response;
        }
    }
}
