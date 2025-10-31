using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories.Implementations;
using InventoryService.Infrastructure.Repositories.Interfaces;
using Application.Services.Implementations;
using Application.Services.Interfaces;  
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using InventoryService.API.Controllers.Mapping;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);
//builder.WebHost.UseUrls("http://localhost:7001");
builder.WebHost.UseUrls("http://0.0.0.0:7001");
var app = builder.Build();

//if (app.Environment.IsDevelopment())
if(true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
