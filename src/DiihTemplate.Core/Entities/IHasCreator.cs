namespace DiihTemplate.Core.Entities;

public interface IHasCreator<TKey>
{
    TKey? CreatorId { get; set; }
}

public interface IHasCreator<TKey, TEntity> : IHasCreator<TKey>
{
    TEntity? Creator { get; set; }
}