var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.WebHost.UseUrls("http://localhost:7000");


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.MapReverseProxy();

app.Run();
