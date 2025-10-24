using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories.Interfaces;
using UserService.Infrastructure.Repositories.Implementations;
using UserService.API.Mapping;
using AutoMapper;
using Application.Services.Interfaces;
using Application.Services.Implementations;
using Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, Application.Services.Implementations.UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<UserProfile>());

builder.Services.AddSingleton<JwtHelper>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new JwtHelper(config);
});

builder.WebHost.UseUrls("http://localhost:7003");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
