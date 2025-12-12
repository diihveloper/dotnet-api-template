using DiihTemplate.Core.Events;

namespace DiihTemplate.Core.Entities;

public interface IHasEvents
{
    IEnumerable<IDomainEvent> Events { get; }
    void ClearEvents();
}