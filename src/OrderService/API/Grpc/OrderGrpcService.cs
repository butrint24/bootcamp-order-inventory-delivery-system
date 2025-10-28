using Application.Services.Interfaces;
using Grpc.Core;
using OrderService.Grpc;

namespace API.Grpc
{
    public class OrderGrpcService : OrderService.Grpc.OrderService.OrderServiceBase
    {
        private readonly IOrderService _orderService;

        public OrderGrpcService(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public override Task<UpdateOrderStatusResponse> UpdateOrderStatus(UpdateOrderStatusRequest request, ServerCallContext context)
        {
            
        }
    }
}