var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
app.MapGet("/", () => "InventoryService running ✅");
app.MapGet("/health", () => Results.Ok("OK"));

{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
