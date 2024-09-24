var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy
    .LoadFromConfig

var app = builder.Build();


app.Run();

