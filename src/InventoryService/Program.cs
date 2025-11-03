using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories.Implementations;
using InventoryService.Infrastructure.Repositories.Interfaces;
using Application.Services.Implementations;
using Application.Services.Interfaces;  
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using InventoryService.API.Controllers.Mapping;
using InventoryService.API.Grpc;
using InventoryService.Application.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddJsonFile("../Shared/appsettings.Production.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

string GetServiceUrl(string name, string defaultUrl) =>
    Environment.GetEnvironmentVariable($"{name.ToUpper()}_URL") 
    ?? builder.Configuration[$"ServiceUrls:{name}"] 
    ?? defaultUrl;

var orderServiceUrl = GetServiceUrl("Orders", "http://localhost:7002");

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddGrpc();

builder.Services.AddGrpcClient<OrderService.GrpcGenerated.OrderService.OrderServiceClient>(o =>
{
    o.Address = new Uri(orderServiceUrl);
});

builder.Services.AddScoped<OrderGrpcClient>();  

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(7001, listenOptions =>
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

app.MapGrpcService<ProductGrpcService>();
app.MapControllers();

app.Run();
