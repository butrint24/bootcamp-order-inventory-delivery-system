using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Application.Services.Interfaces;
using InventoryService.GrpcGenerated;
using InventoryService.Application.Clients;

namespace InventoryService.API.Grpc
{
    public class ProductGrpcService : ProductService.ProductServiceBase
    {
        private readonly IProductService _productService;
        private readonly OrderGrpcClient _orderClient;
        private readonly ILogger<ProductGrpcService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ProductGrpcService(
            IProductService productService,
            OrderGrpcClient orderClient,
            ILogger<ProductGrpcService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _productService = productService;
            _orderClient = orderClient;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
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

        public override async Task<RollbackProductsResponse> RollbackProducts(RollbackProductsMessage request, ServerCallContext context)
        {
            try
            {
                foreach (var (productId, quantity) in request.IdsAndQuantities)
                {
                    await _productService.RollbackProductStockAsync(Guid.Parse(productId), quantity);
                }

                await _productService.SaveChangesAsync();

                _logger.LogInformation("Rollback successful for products: {Products}", string.Join(", ", request.IdsAndQuantities.Keys));

                return new RollbackProductsResponse
                {
                    Success = true,
                    Message = "Products successfully returned"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rollback products: {Products}", string.Join(", ", request.IdsAndQuantities.Keys));
                return new RollbackProductsResponse
                {
                    Success = false,
                    Message = "Failed to process return due to an internal error"
                };
            }
        }

        private async Task ConfirmOrRollbackAsync(Guid orderId, List<GrpcProduct> reservedProducts)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            try
            {
                await Task.Delay(2000);
                var persisted = await _orderClient.IsOrderPersistedAsync(orderId);

                if (!persisted)
                {
                    foreach (var product in reservedProducts)
                        await productService.RollbackProductStockAsync(Guid.Parse(product.ProductId), product.BoughtStock);

                    await productService.SaveChangesAsync();
                }

                _logger.LogInformation("Order {OrderId} persisted: {Persisted}", orderId, persisted);
            }
            catch (Exception ex)
            {
                foreach (var product in reservedProducts)
                    await productService.RollbackProductStockAsync(Guid.Parse(product.ProductId), product.BoughtStock);

                await productService.SaveChangesAsync();
                _logger.LogError(ex, "Order check failed for OrderId: {OrderId}", orderId);
            }
        }

        public override async Task<RestockProductsResponse> RestockProducts(RestockProductsMessage request, ServerCallContext context)
        {
            bool allSucceeded = true;
            foreach (var (productId, quantity) in request.IdsAndQuantities)
            {
                try
                {
                    var result = await _productService.RestockProductStockAsync(Guid.Parse(productId), quantity);
                    if (!result)
                    {
                        _logger.LogWarning("Failed to restock ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);
                        allSucceeded = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while restocking ProductId: {ProductId}", productId);
                    allSucceeded = false;
                }
            }

            var response = new RestockProductsResponse { Success = allSucceeded };

            _ = Task.Run(() => CheckOrderCancelation(Guid.Parse(request.OrderId), request));

            return response;
        }

        private async Task CheckOrderCancelation(Guid orderId, RestockProductsMessage reservedProducts)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            try
            {
                await Task.Delay(2000);
                var isCanceled = await _orderClient.IsOrderCanceledAsync(orderId);

                if (!isCanceled)
                {
                    foreach (var (productId, quantity) in reservedProducts.IdsAndQuantities)
                        await productService.RollbackProductStockAsync(Guid.Parse(productId), quantity);

                    await productService.SaveChangesAsync();
                }

                _logger.LogInformation("Order {OrderId} canceled: {IsCanceled}", orderId, isCanceled);
            }
            catch (Exception ex)
            {
                foreach (var (productId, quantity) in reservedProducts.IdsAndQuantities)
                    await productService.RollbackProductStockAsync(Guid.Parse(productId), quantity);

                await productService.SaveChangesAsync();
                _logger.LogError(ex, "Order cancelation check failed for OrderId: {OrderId}", orderId);
            }
        }
    }
}