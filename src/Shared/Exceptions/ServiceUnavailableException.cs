namespace Shared.Exceptions
{
    public class ServiceUnavailableException : AppException
    {
        public ServiceUnavailableException(string message)
            : base(message, 503) { }
    }
}
