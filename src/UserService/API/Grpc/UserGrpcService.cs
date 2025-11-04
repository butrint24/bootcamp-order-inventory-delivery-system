using Grpc.Core;
using UserService.GrpcGenerated;
using Application.Services.Interfaces;
using System;
using System.Threading.Tasks;


namespace UserService.API.Grpc
{
    public class UserGrpcService : UserService.GrpcGenerated.UserService.UserServiceBase
    {
        private readonly IUserService _userService;

        public UserGrpcService(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<UserValidationResponse> ValidateUser(UserValidationMessage request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid User ID format."));
            }

            Shared.Enums.RoleType? requiredRole =
                 Enum.TryParse<Shared.Enums.RoleType>(request.Role, true, out var parsedRole)
                     ? parsedRole
                     : (Shared.Enums.RoleType?)null;

            var isValid = await _userService.ValidateUserAsync(userId, requiredRole);

            return new UserValidationResponse
            {
                Validated = isValid
            };
        }
        
        public override async Task<UserInfoMessage> GetUserInfo(UserIdMessage request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid User ID format."));
            }

            var userInfo = await _userService.GetByIdAsync(userId);
            if (userInfo == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found."));
            }

            return new UserInfoMessage
            {
                Email = userInfo.Email,
                Address = userInfo.Address,
                Tel = userInfo.Tel
            };
        }
    }
}