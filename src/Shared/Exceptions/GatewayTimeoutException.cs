namespace Shared.Exceptions
{
    public class GatewayTimeoutException : AppException
    {
        public GatewayTimeoutException(string message)
            : base(message, 504) { }
    }
}
