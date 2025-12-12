using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiihTemplate.Core.Events;

public class ApplicationEventService : BackgroundService
{
    private readonly Channel<IApplicationEvent> _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ApplicationEventService> _logger;

    public ApplicationEventService(Channel<IApplicationEvent> channel, IServiceScopeFactory serviceScopeFactory,
        ILogger<ApplicationEventService> logger)
    {
        _channel = channel;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _channel.Reader.WaitToReadAsync(cancellationToken: stoppingToken))
        {
            try
            {
                var domainEvent = await _channel.Reader.ReadAsync(stoppingToken);
                await HandleAsync(domainEvent);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Erro crítico em um dos manipuladores de eventos");
            }
        }
    }

    protected async Task HandleAsync(IApplicationEvent domainEvent)
    {
        var domainEventType = domainEvent.GetType();
        var handlerType = typeof(IApplicationEventHandler<>).MakeGenericType(domainEventType);
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            // var handler =
            //     scope.ServiceProvider.GetRequiredService(handlerType) as IApplicationEventHandler<IApplicationEvent>;
            var handlers = scope.ServiceProvider.GetServices(handlerType).ToList();
            if (handlers.Count == 0)
            {
                _logger.LogWarning("Nenhum manipulador encontrado para o tipo de evento {DomainEventType}",
                    domainEventType);
                return;
            }

            foreach (var handler in handlers)
            {
                var handleMethod = handlerType.GetMethod("HandleAsync");
                if (handleMethod == null)
                {
                    _logger.LogWarning("Método HandleAsync não encontrado para o tipo de evento {DomainEventType}",
                        domainEventType);
                    return;
                }

                await ((Task)handleMethod.Invoke(handler, [domainEvent])!).ConfigureAwait(false);
                await Task.Yield();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao processar evento da aplicação. Tipo de evento: {DomainEventType}",
                domainEventType.Name);
        }
    }
}