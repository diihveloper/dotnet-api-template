#if RABBITMQ
using System.Text;
using System.Text.Json;
using DiihTemplate.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DiihTemplate.Infra.Messaging;

public class RabbitMqMessageBroker : IMessageBroker
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqMessageBroker> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqMessageBroker(IOptions<RabbitMqSettings> options, ILogger<RabbitMqMessageBroker> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string? exchange = null, string? routingKey = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var channel = await GetChannelAsync(cancellationToken);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var targetExchange = exchange ?? _settings.DefaultExchange ?? string.Empty;
        var targetRoutingKey = routingKey ?? typeof(T).Name.ToLowerInvariant();

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(targetExchange, targetRoutingKey, false, properties, body,
            cancellationToken);

        _logger.LogInformation("Published message to exchange {Exchange} with routing key {RoutingKey}",
            targetExchange, targetRoutingKey);
    }

    public async Task SubscribeAsync<T>(string queue, Func<T, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default) where T : class
    {
        var channel = await GetChannelAsync(cancellationToken);
        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.Span);
                var message = JsonSerializer.Deserialize<T>(body);

                if (message is not null)
                {
                    await handler(message, cancellationToken);
                    await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from queue {Queue}", queue);
                await channel.BasicNackAsync(ea.DeliveryTag, false, true, cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Subscribed to queue {Queue}", queue);
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
            return _channel;

        if (_connection is not { IsOpen: true })
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };
            _connection = await factory.CreateConnectionAsync(cancellationToken);
        }

        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();
        if (_connection is not null)
            await _connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
#endif
