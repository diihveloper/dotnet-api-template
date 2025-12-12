namespace DiihTemplate.Core.Entities;

public interface IHasDeletedTime : ISoftDelete
{
    DateTime? DeletedAt { get; set; }
}