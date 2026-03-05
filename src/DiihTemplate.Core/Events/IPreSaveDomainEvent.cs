namespace DiihTemplate.Core.Events;

/// <summary>
/// Marker interface for domain events that should be dispatched BEFORE SaveChanges commits.
/// Use for side effects that must participate in the same transaction.
/// </summary>
public interface IPreSaveDomainEvent : IDomainEvent;
