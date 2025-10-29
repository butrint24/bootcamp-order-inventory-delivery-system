using DeliveryService.Application.Services.Interfaces;
using DeliveryService.GrpcGenerated;
using Grpc.Core;

namespace DeliveryService.API.Grpc
{
    public class DeliveryGrpcService : DeliveryService.GrpcGenerated.DeliveryService.DeliveryServiceBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryGrpcService(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public override async Task<CreateDeliveryResponse> createDelivery(CreateDeliveryMessage request, ServerCallContext context)
        {
            if (request.OrderId == null || request.UserId == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "OrderId and UserId cannot be null."));
            }
            if (!Guid.TryParse(request.OrderId, out var orderId) || !Guid.TryParse(request.UserId, out var userId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID format for OrderId or UserId."));
            }

            var response = await _deliveryService.CreateDeliveryGrpcAsync(orderId, userId);
            return response;
        }

    }
}
