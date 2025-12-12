using System.Linq.Expressions;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DiihTemplate.Core.Repositories;

public class Repository<TEntity, TKey> : ReadOnlyRepository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public Repository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().AsNoTracking().FirstAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken) ??
               throw new EntityNotFoundException();
    }

    public Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<TEntity>().AnyAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(id, cancellationToken);
        DbContext.Set<TEntity>().Remove(entity);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbContext.Set<TEntity>().Add(entity);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbContext.Set<TEntity>().AddRange(entities);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbContext.Set<TEntity>().Update(entity);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbContext.Set<TEntity>().UpdateRange(entities);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        DbContext.Set<TEntity>().Remove(entity);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<TEntity>().Where(predicate).ExecuteDeleteAsync(cancellationToken);
    }
}

public class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity> where TEntity : class, IEntity
{
    DbContext _dbContext;

    public Repository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        if (entity is IHasCreationTime hasCreationTime)
        {
            hasCreationTime.CreatedAt = DateTime.UtcNow;
        }

        _dbContext.Set<TEntity>().Add(entity);
        if (autoSave)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        entities = entities.ToList();
        if (entities.Any(x => x is IHasCreationTime))
        {
            foreach (var entity in entities)
            {
                if (entity is IHasCreationTime hasCreationTime)
                {
                    hasCreationTime.CreatedAt = DateTime.UtcNow;
                }
            }
        }

        _dbContext.Set<TEntity>().AddRange(entities);
        if (autoSave)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        if (entity is IHasUpdatedTime hasModificationTime)
        {
            hasModificationTime.UpdatedAt = DateTime.UtcNow;
        }

        _dbContext.Set<TEntity>().Update(entity);
        if (autoSave)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        entities = entities.ToList();
        foreach (var entity in entities)
        {
            if (entity is IHasUpdatedTime hasModificationTime)
            {
                hasModificationTime.UpdatedAt = DateTime.UtcNow;
            }
        }

        _dbContext.Set<TEntity>().UpdateRange(entities);
        if (autoSave)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        if (entity is ISoftDelete softDelete)
        {
            softDelete.IsDeleted = true;
            if (softDelete is IHasDeletedTime hasDeletionTime)
            {
                hasDeletionTime.DeletedAt = DateTime.UtcNow;
            }

            return UpdateAsync(entity, autoSave, cancellationToken);
        }

        _dbContext.Set<TEntity>().Remove(entity);
        if (autoSave)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            if (typeof(IHasDeletedTime).IsAssignableFrom(typeof(TEntity)))
            {
                return _dbContext.Set<TEntity>().Where(predicate)
                    .ExecuteUpdateAsync(x => x
                        .SetProperty(e => ((e as ISoftDelete)!).IsDeleted, true)
                        .SetProperty(e => ((e as IHasDeletedTime)!).DeletedAt, DateTime.UtcNow), cancellationToken);
            }

            return _dbContext.Set<TEntity>().Where(predicate)
                .ExecuteUpdateAsync(x => x.SetProperty(e => ((e as ISoftDelete)!).IsDeleted, true), cancellationToken);
        }

        return _dbContext.Set<TEntity>().Where(predicate).ExecuteDeleteAsync(cancellationToken);
    }
}