using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;
if (env == "Production")
    builder.Configuration.AddJsonFile("appsettings.Deploy.json", optional: false, reloadOnChange: true);
else
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Configuration.AddJsonFile("appsettings.Secrets.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(o =>
{
    o.AddPolicy("FrontOnly", p => p
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<JwtHelper>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new JwtHelper(config);
});

builder.Services.AddReverseProxy()
       .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.WebHost.UseUrls("http://0.0.0.0:7000");

var app = builder.Build();

app.UseCors("Localhost3000");

app.UseCors("FrontOnly");

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/auth", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }

    var jwtHelper = context.RequestServices.GetRequiredService<JwtHelper>();
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

    if (!string.IsNullOrEmpty(token))
    {
        var principal = jwtHelper.ValidateToken(token);
        if (principal != null)
        {
            var userId = principal.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            var role = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userId))
                context.Request.Headers["X-User-Id"] = userId;

            if (!string.IsNullOrEmpty(role))
                context.Request.Headers["X-User-Role"] = role;

            await next();
            return;
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid or expired token.");
            return;
        }
    }

    context.Response.StatusCode = 401;
    await context.Response.WriteAsync("Missing token.");
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapReverseProxy();

app.Run();