using DeliveryService.Application.Services.Interfaces;
using DeliveryService.GrpcGenerated;
using Grpc.Core;

namespace DeliveryService.API.Grpc;

public class DeliveryGrpcService(IDeliveryService deliveryService)
    : DeliveryService.GrpcGenerated.DeliveryService.DeliveryServiceBase
{
    public override async Task<CreateDeliveryResponse> createDelivery(CreateDeliveryMessage request, ServerCallContext context)
    {
        if (request.OrderId == null || request.UserId == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "OrderId and UserId cannot be null."));

        if (!Guid.TryParse(request.OrderId, out var orderId) || !Guid.TryParse(request.UserId, out var userId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID format for OrderId or UserId."));

        var response = await deliveryService.CreateDeliveryGrpcAsync(orderId, userId);
        return response;
    }

    public override async Task<CancelDeliveryResponse> CancelDelivery(CancelDeliveryMessage request, ServerCallContext context)
    {
        if (request.OrderId == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "OrderId cannot be null."));

        if (!Guid.TryParse(request.OrderId, out var orderId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID format for OrderId."));

        var response = await deliveryService.CancelDeliveryGrpcAsync(orderId);
        CancelDeliveryResponse cancelDeliveryMessage = new CancelDeliveryResponse
        {
            Success = response
        };
        return cancelDeliveryMessage;
    }
}
