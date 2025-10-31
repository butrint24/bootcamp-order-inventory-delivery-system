using Grpc.Core;
using OrderService.GrpcGenerated;
using Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GrpcService = OrderService.GrpcGenerated.OrderService;

namespace OrderService.API.Grpc
{
    public class OrderGrpcService : GrpcService.OrderServiceBase
    {
        private readonly IOrderService _service;
        private readonly ILogger<OrderGrpcService> _logger;

        public OrderGrpcService(IOrderService service, ILogger<OrderGrpcService> logger)
        {
            _service = service;
            _logger = logger;
        }

        public override async Task<GetUserOrdersResponse> GetUserOrders(GetUserOrdersRequest request, ServerCallContext context)
        {
            _logger.LogInformation("--- gRPC request received for User ID: {UserId} ---", request.UserId);

            var orderDtos = await _service.GetOrdersForUserAsync(request.UserId);

            var response = new GetUserOrdersResponse();

            foreach (var dto in orderDtos)
            {
                response.Orders.Add(new OrderMessage
                {
                    OrderId = dto.OrderId.ToString(),
                    Status = dto.Status.ToString(),
                    Price = (double)dto.Price
                });
            }

            _logger.LogInformation("--- Found {OrderCount} orders for User ID: {UserId} ---", response.Orders.Count, request.UserId);

            return response;
        }
    }
}