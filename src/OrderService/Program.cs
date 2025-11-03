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

builder.Configuration
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile("../Shared/appsettings.Production.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();

string GetServiceUrl(string name, string defaultUrl) =>
    Environment.GetEnvironmentVariable($"{name.ToUpper()}_URL") 
    ?? builder.Configuration[$"ServiceUrls:{name}"] 
    ?? defaultUrl;

var deliveryServiceUrl = GetServiceUrl("Delivery", "http://localhost:7004");
var userServiceUrl = GetServiceUrl("Users", "http://localhost:7003");
var inventoryServiceUrl = GetServiceUrl("Inventory", "http://localhost:7001");

var env = builder.Environment.EnvironmentName;
string connectionString = env == "Production"
    ? builder.Configuration.GetConnectionString("ProdConnection")
    : builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(connectionString)
           .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
);

builder.Services.AddScoped<IOrderService, Application.Services.Implementations.OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddGrpc();

builder.Services.AddGrpcClient<DeliveryService.GrpcGenerated.DeliveryService.DeliveryServiceClient>(o =>
{
    o.Address = new Uri(deliveryServiceUrl);
});
builder.Services.AddGrpcClient<UserService.GrpcGenerated.UserService.UserServiceClient>(o =>
{
    o.Address = new Uri(userServiceUrl);
});
builder.Services.AddGrpcClient<InventoryService.GrpcGenerated.ProductService.ProductServiceClient>(o =>
{
    o.Address = new Uri(inventoryServiceUrl);
});

builder.Services.AddScoped<UserGrpcClient>();
builder.Services.AddScoped<ProductClient>();
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
