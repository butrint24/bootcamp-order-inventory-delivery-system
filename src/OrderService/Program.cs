using Application.Services.Implementations;
using Application.Services.Interfaces;
using OrderService.Infrastructure.Repositories.Interfaces;
using OrderService.Infrastructure.Repositories.Implementations;
using OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IOrderService, Application.Services.Implementations.OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://localhost:7002");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGet("/Order/hello", () =>
{
    var port = app.Urls.FirstOrDefault()?.Split(':').Last() ?? "unknown";
    return $"Hello from {port}";
});

app.Run();
