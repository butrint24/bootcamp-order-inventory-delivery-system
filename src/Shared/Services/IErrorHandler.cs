using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface IErrorHandler
    {
        Task HandleAsync(HttpContext context, Exception ex);
    }
}
