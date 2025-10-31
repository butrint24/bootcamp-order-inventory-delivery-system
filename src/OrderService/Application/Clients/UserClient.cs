using UserService.GrpcGenerated;

namespace OrderService.Application.Clients
{
    public class UserGrpcClient
    {
        private readonly UserService.GrpcGenerated.UserService.UserServiceClient _client;

        public UserGrpcClient(UserService.GrpcGenerated.UserService.UserServiceClient client)
        {
            _client = client;
        }

        public async Task<UserValidationResponse> ValidateUserAsync(Guid userId, string role)
        {
            var request = new UserValidationMessage
            {
                UserId = userId.ToString(),
                Role = role
            };

            return await _client.ValidateUserAsync(request);
        }

        public async Task<UserInfoMessage> GetUserInfoAsync(Guid userId)
        {
            var request = new UserIdMessage
            {
                UserId = userId.ToString()
            };

            return await _client.GetUserInfoAsync(request);
        }
    }
}