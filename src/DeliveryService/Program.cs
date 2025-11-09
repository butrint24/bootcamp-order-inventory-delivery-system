using DeliveryService.Infrastructure.Data;
using DeliveryService.Infrastructure.Repositories.Implementations;
using DeliveryService.Infrastructure.Repositories.Interfaces;
using DeliveryService.Application.Services.Implementations;
using DeliveryService.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using DeliveryService.API.Mapping;
using DeliveryService.Application.Clients;
using DeliveryService.API.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();

string GetServiceUrl(string name, string defaultUrl) =>
    Environment.GetEnvironmentVariable($"{name.ToUpper()}_URL") 
    ?? builder.Configuration[$"ServiceUrls:{name}"] 
    ?? defaultUrl;

var env = builder.Environment.EnvironmentName;
string connectionString = env == "Production"
    ? builder.Configuration.GetConnectionString("ProdConnection")
    : builder.Configuration.GetConnectionString("DefaultConnection");

var orderServiceUrl = env == "Production" ? GetServiceUrl("Order", "http://localhost:7002") : "http://localhost:7002";

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DeliveryDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddGrpc();
builder.Services.AddScoped<IDeliveryService, DeliveryService.Application.Services.Implementations.DeliveryService>();
builder.Services.AddHostedService<DeliverySchedulerService>();
builder.Services.AddAutoMapper(typeof(DeliveryProfile).Assembly);

builder.Services.AddGrpcClient<OrderService.GrpcGenerated.OrderService.OrderServiceClient>(o =>
{
    o.Address = new Uri(orderServiceUrl);
});
builder.Services.AddScoped<OrderGrpcClient>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7004, listenOptions =>
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

app.MapGrpcService<DeliveryGrpcService>();
app.MapControllers();

app.Run();