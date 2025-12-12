namespace DiihTemplate.Core.Entities;

public interface IHasDeleter<TKey> : ISoftDelete
{
    TKey? DeleterId { get; set; }
}

public interface IHasDeleter<TKey, TEntity> : IHasDeleter<TKey>
{
    TEntity? Deleter { get; set; }
}