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


builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DeliveryDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddGrpc();
builder.Services.AddScoped<IDeliveryService, DeliveryService.Application.Services.Implementations.DeliveryService>();
builder.Services.AddHostedService<DeliverySchedulerService>();
builder.Services.AddAutoMapper(typeof(DeliveryProfile).Assembly);
<<<<<<< HEAD
<<<<<<< HEAD
builder.Services.AddGrpcClient<OrderService.GrpcGenerated.OrderService.OrderServiceClient>(o =>
{
    o.Address = new Uri("http://localhost:7002");
});
builder.Services.AddScoped<OrderGrpcClient>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(7004, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});
=======
// builder.WebHost.UseUrls("http://localhost:7004");
builder.WebHost.UseUrls("http://0.0.0.0:7004");
>>>>>>> e989525 (Add Kubernetes local setup for InventoryService with Postgres)
=======
builder.WebHost.UseUrls("http://localhost:7004");

>>>>>>> 77c9f8b (WIP: K8s tweaks (Program.cs for URLs/ports))

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGrpcService<DeliveryGrpcService>();
app.MapControllers();

app.Run();
