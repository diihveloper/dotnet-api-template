namespace DiihTemplate.Core.Events;

public interface IApplicationEventHandler<T> where T : IApplicationEvent
{
    Task HandleAsync(T domainEvent);
}