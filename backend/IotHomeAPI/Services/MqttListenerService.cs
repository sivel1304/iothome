using MQTTnet;
using IotHomeAPI.Data;
using IotHomeAPI.Models;
using IotHomeAPI.Validation;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using IotHomeAPI.Dtos;

namespace IotHomeAPI.Services;

public class MqttListenerService : BackgroundService
{
    private readonly ILogger<MqttListenerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<SensorHub> _hub;

    public MqttListenerService(ILogger<MqttListenerService> logger, IServiceScopeFactory scopeFactory, IHubContext<SensorHub> sensorHub)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _hub = sensorHub;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttClientFactory();

        using var mqttClient = mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("192.168.0.3", 1883).WithClientId($"iothome-api-{Guid.NewGuid():N}").Build();
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic("viggo-home/#"))   // # = wildcard, all sensors/topics
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            string topic = e.ApplicationMessage.Topic;
            string payload = e.ApplicationMessage.ConvertPayloadToString();

            _logger.LogInformation("Received {Topic}: {Payload}", topic, payload);

            var parts = topic.Split('/');
            string moduleId = parts.Length > 1 ? parts[1] : "unknown";

            if (!ModulePayloadParser.TryParse(payload, out var message) || message is null
            || message.Readings.Count is 0 or > 20)
            {
                _logger.LogWarning("Ignoring invalid payload from {ModuleId}: {Payload}", moduleId, payload);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IotHomeDbContext>();

            DateTime now = DateTime.UtcNow;


            var module = await db.Modules.FindAsync(moduleId);
            if (module is null)
            {
                module = new Module { Id = moduleId, Name = moduleId };
                db.Modules.Add(module);
            }

            module.IntervalMs = message.Interval;
            module.LastSeen = now;

            db.Measurements.Add(new Measurement
            {
                Module = module,
                Timestamp = now,
                Readings = message.Readings
                    .Select(kv => new Reading { Type = kv.Key, Value = kv.Value })
                    .ToList()
            });


            await db.SaveChangesAsync();
            var readings = DerivedReadings.WithDerived(
            message.Readings.Select(kv => new ReadingDto(kv.Key, kv.Value)));

            await _hub.Clients.All.SendAsync("measurement",
            new LatestMeasurementDto(moduleId, now, readings));
        };


        // Reconnecting is left to the loop below, which also resubscribes
        // (the broker drops subscriptions on disconnect with a clean session).
        mqttClient.DisconnectedAsync += e =>
        {
            _logger.LogWarning("MQTT disconnected, retrying in 5s...");
            return Task.CompletedTask;
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!mqttClient.IsConnected)
            {
                try
                {
                    await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);
                    await mqttClient.SubscribeAsync(mqttSubscribeOptions, stoppingToken);
                    _logger.LogInformation("Connected and subscribed to viggo-home/#");
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogWarning("MQTT connect failed: {Message}. Retrying in 5s...", ex.Message);
                }
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}