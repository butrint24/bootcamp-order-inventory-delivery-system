using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Application.Services.Interfaces;
using Shared.DTOs;
using InventoryService.GrpcGenerated;
using InventoryService.Application.Clients;
using Microsoft.Extensions.Logging;

namespace InventoryService.API.Grpc
{
    public class ProductGrpcService : InventoryService.GrpcGenerated.ProductService.ProductServiceBase
    {
        private readonly IProductService _productService;
        private readonly OrderGrpcClient _orderClient;
        private readonly ILogger<ProductGrpcService> _logger;

        public ProductGrpcService(
            IProductService productService, 
            OrderGrpcClient orderClient,
            ILogger<ProductGrpcService> logger)
        {
            _productService = productService;
            _orderClient = orderClient;
            _logger = logger;
        }

        public override async Task<GetProductsResponse> GetProducts(BuyProductsMessage request, ServerCallContext context)
        {
            _logger.LogInformation("Received GetProducts request for OrderId: {OrderId}", request.OrderId);
            foreach (var kvp in request.IdsAndQuantities)
            {
                _logger.LogInformation("Requested ProductId: {ProductId}, Quantity: {Quantity}", kvp.Key, kvp.Value);
            }

            var reservedProducts = new List<GrpcProduct>();

            foreach (var (productId, quantity) in request.IdsAndQuantities)
            {
                try
                {
                    var product = await _productService.ReserveProductStock(Guid.Parse(productId), quantity);
                    reservedProducts.Add(product);
                    _logger.LogInformation("Reserved ProductId: {ProductId}, Quantity: {Quantity}, StockAfterReservation: {Stock}", productId, quantity, product.BoughtStock);
                }
                catch (KeyNotFoundException)
                {
                    _logger.LogWarning("ProductId not found: {ProductId}", productId);
                    return new GetProductsResponse { Success = false };
                }
                catch (InvalidOperationException)
                {
                    _logger.LogWarning("Cannot reserve ProductId: {ProductId}, requested quantity: {Quantity}", productId, quantity);
                    return new GetProductsResponse { Success = false };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while reserving ProductId: {ProductId}", productId);
                    return new GetProductsResponse { Success = false };
                }
            }

            var response = new GetProductsResponse
            {
                Success = true,
                GrpcProducts = { reservedProducts }
            };

            _logger.LogInformation("Sending GetProducts response for OrderId: {OrderId} with {Count} products", request.OrderId, reservedProducts.Count);

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(2000);
                    var persisted = await _orderClient.IsOrderPersistedAsync(Guid.Parse(request.OrderId));
                    _logger.LogInformation("Inside Time is: {Time}", DateTime.UtcNow);
                    _logger.LogInformation("Order {OrderId} persisted: {Persisted}", request.OrderId, persisted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Order check failed for OrderId: {OrderId}", request.OrderId);
                }
            });

            _logger.LogInformation("Outside time is: {Time}", DateTime.UtcNow);
            return response;
        }
    }
}
