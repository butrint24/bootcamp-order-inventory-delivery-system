using Application.Services.Interfaces;
using Grpc.Core;
using InventoryService.GrpcGenerated;
using System;
using System.Threading.Tasks;

namespace InventoryService.API.Grpc
{
    public class InventoryGrpcService : InventoryService.GrpcGenerated.InventoryService.InventoryServiceBase
    {
        private readonly IProductService _productService;

        public InventoryGrpcService(IProductService productService)
        {
            _productService = productService;
        }

        public override async Task<DecreaseStockResponse> DecreaseStock(DecreaseStockRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.ProductId, out var productId))
            {
                return new DecreaseStockResponse
                {
                    Success = false,
                    Message = "Invalid product ID format."
                };
            }

            if (request.Quantity <= 0)
            {
                return new DecreaseStockResponse
                {
                    Success = false,
                    Message = "Quantity must be greater than 0."
                };
            }

            try
            {
                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                {
                    return new DecreaseStockResponse
                    {
                        Success = false,
                        Message = "Product not found."
                    };
                }

                if (product.Stock < request.Quantity)
                {
                    return new DecreaseStockResponse
                    {
                        Success = false,
                        Message = "Insufficient stock."
                    };
                }

                
                product.Stock -= request.Quantity;

                
                var updateDto = new Shared.DTOs.ProductUpdateDto
                {
                    Stock = product.Stock
                };

                await _productService.UpdateProductAsync(productId, updateDto);

                return new DecreaseStockResponse
                {
                    Success = true,
                    Message = $"Stock decreased by {request.Quantity} for product {productId}."
                };
            }
            catch (Exception ex)
            {
                return new DecreaseStockResponse
                {
                    Success = false,
                    Message = $"Error decreasing stock: {ex.Message}"
                };
            }
        }
    }
}
