using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models;

namespace Shared.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.Request.Headers.TryGetValue("X-Trace-Id", out var traceHeader)
                ? traceHeader.ToString()
                : "Unknown";
            var user = context.Request.Headers.TryGetValue("X-User-Id", out var userId)
                ? userId.ToString()
                : "Anonymous";

            var (statusCode, title, detail, errors) = MapException(exception);

            var response = new ProblemDetailsResponse
            {
                Type = $"https://httpstatuses.io/{statusCode}",
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = context.Request.Path,
                TraceId = traceId,
                User = user,
                Errors = errors
            };

            _logger.LogError(exception,
                "[{Status}] {Title}: {Detail} | TraceId: {TraceId} | User: {User}",
                statusCode, title, detail, traceId, user);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(json);
        }

        private static (int status, string title, string detail, object? errors)
            MapException(Exception ex)
        {
            return ex switch
            {
                ValidationException v => (400, "Validation Error", v.Message, v.Errors),
                BadRequestException b => (400, "Bad Request", b.Message, null),
                UnauthorizedException u => (401, "Unauthorized", u.Message, null),
                ForbiddenException f => (403, "Forbidden", f.Message, null),
                NotFoundException n => (404, "Not Found", n.Message, null),
                ConflictException c => (409, "Conflict", c.Message, null),
                DomainException d => (422, "Domain Error", d.Message, null),
                ServiceUnavailableException s => (503, "Service Unavailable", s.Message, null),
                GatewayTimeoutException g => (504, "Gateway Timeout", g.Message, null),
                InternalServerErrorException i => (500, "Internal Server Error", i.Message, null),
                AppException a => (a.StatusCode, a.GetType().Name, a.Message, null),
                _ => (500, "Internal Server Error", "An unexpected error occurred.", null)
            };
        }
    }
}
