using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Shared.Services
{
    public class ErrorHandler : IErrorHandler
    {
        public async Task HandleAsync(HttpContext context, Exception ex)
        {
            Console.WriteLine($"[ErrorHandler] {ex.GetType().Name}: {ex.Message}");

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var traceId = context.Request.Headers.TryGetValue("X-Trace-Id", out var th) ? th.ToString() : "Unknown";
            var user = context.Request.Headers.TryGetValue("X-User-Id", out var uid) ? uid.ToString() : "Anonymous";
            Console.WriteLine($"[ErrorHandler] {ex.GetType().Name}: {ex.Message} | TraceId: {traceId} | User: {user}");


            var response = new
            {
                success = false,
                message = "An unexpected error occurred.",
                details = ex.Message 
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
