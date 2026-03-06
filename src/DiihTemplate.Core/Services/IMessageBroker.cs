#if RABBITMQ
namespace DiihTemplate.Core.Services;

public interface IMessageBroker : IAsyncDisposable
{
    Task PublishAsync<T>(T message, string? exchange = null, string? routingKey = null,
        CancellationToken cancellationToken = default) where T : class;

    Task SubscribeAsync<T>(string queue, Func<T, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default) where T : class;
}
#endif
