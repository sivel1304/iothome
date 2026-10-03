using IotHomeAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<IotHomeDbContext>(options =>
    options.UseSqlite("Data Source=iothome.db"));
builder.Services.AddHostedService<MqttListenerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/dht11", async (IotHomeDbContext db) =>
{
    var readings = await db.DhtReadings
        .OrderByDescending(r => r.Timestamp)
        .ToListAsync();

    return Results.Ok(readings);
})
.WithName("GetDht11Readings");

app.Run();
