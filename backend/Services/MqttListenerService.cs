using MQTTnet;
using IotHomeAPI.Data;

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
        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("test.mosquitto.org", 1883).Build();

        mqttClient.ApplicationMessageReceivedAsync += async  e =>
        {
            string topic = e.ApplicationMessage.Topic;
            string payload = e.ApplicationMessage.ConvertPayloadToString();

            _logger.LogInformation("Received {Topic}: {Payload}", topic, payload);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IotHomeDbContext>();

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

        await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic("viggo-home/#"))   // # = wildcard, all sensors/topics
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, stoppingToken);

        _logger.LogInformation("Subscribed to viggo-home/#");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}