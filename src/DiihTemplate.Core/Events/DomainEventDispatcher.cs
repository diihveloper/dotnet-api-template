using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiihTemplate.Core.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _serviceProvider.GetServices(handlerType).ToList();
            foreach (var handler in handlers)
            {
                try
                {
                    var handleMethod = handlerType.GetMethod("HandleAsync");
                    await ((Task)handleMethod?.Invoke(handler, [domainEvent])!);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Erro ao processar evento de domínio");
                }
            }
        }
    }
}