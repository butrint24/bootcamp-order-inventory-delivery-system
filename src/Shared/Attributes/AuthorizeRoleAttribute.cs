using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Enums; 
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute, IAsyncActionFilter
    {
        private readonly RoleType[] _allowedRoles;

        public AuthorizeRoleAttribute(params RoleType[] roles)
        {
            _allowedRoles = roles;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var roleHeader = context.HttpContext.Request.Headers["X-User-Role"].FirstOrDefault();

            if (!Enum.TryParse<RoleType>(roleHeader, out var userRole) || !_allowedRoles.Contains(userRole))
            {
                context.HttpContext.Response.StatusCode = 403;
                await context.HttpContext.Response.WriteAsync("Forbidden");
                return;
            }


            var userId = context.HttpContext.Request.Headers["X-User-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userId))
            {
                context.HttpContext.Items["UserId"] = userId;
            }

            await next();
        }
    }
}