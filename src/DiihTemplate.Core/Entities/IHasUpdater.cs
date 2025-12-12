namespace DiihTemplate.Core.Entities;

public interface IHasUpdater<TKey>
{
    TKey? UpdaterId { get; set; }
}

public interface IHasUpdater<TKey, TEntity> : IHasUpdater<TKey>
{
    TEntity? Updater { get; set; }
}