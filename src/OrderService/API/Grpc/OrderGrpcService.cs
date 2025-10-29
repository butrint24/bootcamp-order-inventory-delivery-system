using Application.Services.Interfaces;
using Grpc.Core;
using OrderService.GrpcGenerated;
using Shared.Enums;
using System;
using System.Threading.Tasks;

namespace API.Grpc
{
    public class OrderGrpcService : OrderService.GrpcGenerated.OrderService.OrderServiceBase
    {
        private readonly IOrderService _orderService;

        public OrderGrpcService(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public override async Task<UpdateOrderStatusResponse> UpdateOrderStatus(UpdateOrderStatusRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.OrderId, out var orderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Order ID format."));
            }

            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var updatedStatus))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Order Status format."));
            }

            var response = await _orderService.UpdateOrderStatusAsync(orderId, updatedStatus);

            return response;
        }
    }
}
