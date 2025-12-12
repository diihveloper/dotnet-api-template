using System.Linq.Expressions;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DiihTemplate.Core.Repositories;

public class ReadOnlyRepository<TEntity, TKey> : ReadOnlyRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    public ReadOnlyRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().AsNoTracking().FirstAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().FindAsync([id], cancellationToken: cancellationToken) ??
               throw new EntityNotFoundException();
    }

    public async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().AnyAsync(x => x.Id.Equals(id), cancellationToken);
    }
}

public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity
{
    protected DbContext DbContext { get; }

    public ReadOnlyRepository(DbContext dbContext)
    {
        DbContext = dbContext;
    }

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.FirstOrDefaultAsync(predicate, cancellationToken) ??
               throw new EntityNotFoundException();
    }

    public virtual Task<IQueryable<TEntity>> GetQueryableAsync()
    {
        return Task.FromResult(DbContext.Set<TEntity>().AsNoTracking().AsQueryable());
    }

    public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.Where(predicate)
            .ToPagedAsync(page, perPage, cancellationToken);
    }

    public async Task<PagedResult<TEntity>> SearchAsync(string searchTerm, int page, int perPage,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.Search(searchTerm).ToPagedAsync(page, perPage, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.CountAsync(cancellationToken);
    }

    public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.LongCountAsync(predicate, cancellationToken);
    }

    public async Task<long> LongCountAsync(CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.LongCountAsync(cancellationToken);
    }
}