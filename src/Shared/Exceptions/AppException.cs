namespace Shared.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode = 500)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
