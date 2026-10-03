using MQTTnet;
using IotHomeAPI.Data;
using IotHomeAPI.Models;
using System.Text.Json;

public class SensorPayload
{
    public int Temperature { get; set; }
    public int Humidity { get; set; }
}


public class MqttListenerService : BackgroundService
{
    private readonly ILogger<MqttListenerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public MqttListenerService(ILogger<MqttListenerService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttClientFactory();

        using var mqttClient = mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("test.mosquitto.org", 1883).WithClientId($"iothome-api-{Guid.NewGuid():N}").Build();
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic("viggo-home/#"))   // # = wildcard, all sensors/topics
            .Build();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            string topic = e.ApplicationMessage.Topic;
            string payload = e.ApplicationMessage.ConvertPayloadToString();

            _logger.LogInformation("Received {Topic}: {Payload}", topic, payload);

            var reading = JsonSerializer.Deserialize<SensorPayload>(payload, options);
            if (reading is null) return;

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IotHomeDbContext>();

            db.DhtReadings.Add(new DhtReading
            {
                SensorId = topic,
                Temperature = reading.Temperature,
                Humidity = reading.Humidity,
                Timestamp = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        };

        mqttClient.DisconnectedAsync += async e =>
        {
            _logger.LogWarning("MQTT disconnected, retrying in 5s...");
            await Task.Delay(5000, stoppingToken);
            try
            {
                await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT reconnect failed");
            }
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!mqttClient.IsConnected)
            {
                try
                {
                    await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);
                    await mqttClient.SubscribeAsync(mqttSubscribeOptions, stoppingToken);
                    _logger.LogInformation("Connected and subscribed to viggo-home/+");
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