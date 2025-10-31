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

        public override async Task<GetProductsResponse> BuyProducts(BuyProductsMessage request, ServerCallContext context)
        {
            var reservedProducts = new List<GrpcProduct>();

            foreach (var (productId, quantity) in request.IdsAndQuantities)
            {
                try
                {
                    var product = await _productService.ReserveProductStock(Guid.Parse(productId), quantity);
                    reservedProducts.Add(product);
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

            _ = Task.Run(() => ConfirmOrRollbackAsync(Guid.Parse(request.OrderId), reservedProducts));

            return response;
        }

        private async Task ConfirmOrRollbackAsync(Guid orderId, List<GrpcProduct> reservedProducts)
        {
            try
            {
                await Task.Delay(2000);
                var persisted = await _orderClient.IsOrderPersistedAsync(orderId);

                if (!persisted)
                {
                    foreach (var product in reservedProducts)
                        await _productService.RollbackProductStockAsync(Guid.Parse(product.ProductId), product.BoughtStock);

                    await _productService.SaveChangesAsync();
                }

                _logger.LogInformation("Order {OrderId} persisted: {Persisted}", orderId, persisted);
            }
            catch (Exception ex)
            {
                foreach (var product in reservedProducts)
                    await _productService.RollbackProductStockAsync(Guid.Parse(product.ProductId), product.BoughtStock);

                await _productService.SaveChangesAsync();
                _logger.LogError(ex, "Order check failed for OrderId: {OrderId}", orderId);
            }
        }

    }
}
