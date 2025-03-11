using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

var basketMeter = new Meter("Basket.API.Count", "1.0.0");
builder.Services.AddSingleton(basketMeter);

var itemsAddedCounter = basketMeter.CreateCounter<int>(
    "basket.items.added.count",
    description: "Number of items added to baskets");

builder.Services.AddSingleton(itemsAddedCounter);

builder.AddBasicServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<BasketService>();

app.Run();
