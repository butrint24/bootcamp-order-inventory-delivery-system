using Microsoft.AspNetCore.Builder;
using Shared.Middleware;

namespace Shared.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseGlobalErrorHandler(this WebApplication app)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            return app;
        }
    }
}
