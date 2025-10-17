var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://localhost:7002");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/Order/hello", () =>
{
    var port = app.Urls.FirstOrDefault()?.Split(':').Last() ?? "unknown";
    return $"Hello from {port}";
});

app.Run();
