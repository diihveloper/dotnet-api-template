using System.Linq.Expressions;
using DiihTemplate.Core.Entities;

namespace DiihTemplate.Core.Repositories;

public interface IReadOnlyRepository<TEntity> where TEntity : class, IEntity
{
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IQueryable<TEntity>> GetQueryableAsync();

    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, int page, int perPage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Realiza uma busca página por termo nas propriedades como o atributo [Searchable] 
    /// </summary>
    /// <param name="searchTerm">Termo</param>
    /// <param name="page">página</param>
    /// <param name="perPage">items por página</param>
    /// <param name="cancellationToken">token de cancelamento</param>
    /// <returns></returns>
    Task<PagedResult<TEntity>> SearchAsync(string searchTerm, int page, int perPage,
        CancellationToken cancellationToken = default);
    

    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);
}

public interface IReadOnlyRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
}