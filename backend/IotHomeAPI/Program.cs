using IotHomeAPI.Data;
using IotHomeAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using IotHomeAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<IotHomeDbContext>(options =>
    options.UseSqlite("Data Source=iothome.db"));
    builder.Services.AddSignalR();
    builder.Services.AddHostedService<MqttListenerService>();

var app = builder.Build();

app.MapHub<SensorHub>("/hubs/sensors");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Swagger UI at /swagger, reading the built-in OpenAPI document
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "IotHome API"));
}


app.UseHttpsRedirection();


app.MapGet("/latest-reading/{moduleId}", async (string moduleId, IotHomeDbContext db) =>
{
    var latest = await db.Measurements
        .AsNoTracking()
        .Where(m => m.ModuleId == moduleId)
        .OrderByDescending(m => m.Timestamp)
        .Select(m => new LatestMeasurementDto(
            m.ModuleId,
            m.Timestamp,
            m.Readings.Select(r => new ReadingDto(r.Type, r.Value)).ToList()))
        .FirstOrDefaultAsync();

    return latest is null ? Results.NotFound() : Results.Ok(latest);
})
.WithName("GetLatestSensorReading");

app.MapGet("/modules", async (IotHomeDbContext db) =>
{
    var modules = await db.Modules
        .AsNoTracking()
        .OrderBy(m => m.Name)
        .Select(m => new ModuleDto(m.Id, m.Name, m.SensorType, m.IntervalMs, m.LastSeen))
        .ToListAsync();
    return modules is null ? Results.NotFound() : Results.Ok(modules);
})
.WithName("GetModules");

app.MapGet("/modules/{moduleId}/history", async (string moduleId, int? hours, IotHomeDbContext db) =>
{
    if (!await db.Modules.AnyAsync(m => m.Id == moduleId))
        return Results.NotFound();

    int h = Math.Clamp(hours ?? 3, 1, 168);          // default 3 hours, max 7 days
    var since = DateTime.UtcNow.AddHours(-h);

    var measurements = await db.Measurements
        .AsNoTracking()
        .Where(m => m.ModuleId == moduleId && m.Timestamp >= since)
        .OrderBy(m => m.Timestamp)
        .Select(m => new MeasurementDto(
            m.Timestamp,
            m.Readings.Select(r => new ReadingDto(r.Type, r.Value)).ToList()))
        .ToListAsync();

    return Results.Ok(measurements);
})
.WithName("GetModuleHistory");

app.Run();
