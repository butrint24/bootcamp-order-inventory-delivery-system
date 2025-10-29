using Application.Services.Implementations;
using Application.Services.Interfaces;
using OrderService.Infrastructure.Repositories.Interfaces;
using OrderService.Infrastructure.Repositories.Implementations;
using OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Clients;
using Grpc.Net.Client;
using DeliveryService.GrpcGenerated;
using static DeliveryService.GrpcGenerated.DeliveryService;
using API.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
);


builder.Services.AddScoped<IOrderService, Application.Services.Implementations.OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<DeliveryService.GrpcGenerated.DeliveryService.DeliveryServiceClient>(o =>
{
    o.Address = new Uri("http://localhost:7004");
});
builder.Services.AddScoped<DeliveryGrpcClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(7002, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<OrderGrpcService>();
app.MapControllers();

app.Run();
