using System.Linq.Expressions;
using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Entities;

namespace DiihTemplate.Core.Services;

public interface
    IReadOnlyAppService<TEntity, TOutputDto, TListOutputDto, TGetListInput> : IListAppService<TListOutputDto,
    TGetListInput> where TEntity : class, IEntity
{
    Task<TOutputDto?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<TOutputDto> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}

public interface
    IReadOnlyAppService<TEntity, TKey, TGetOutputDto, TGetListOutputDto, TGetListInput> :
    IReadOnlyAppService<TEntity, TGetOutputDto, TGetListOutputDto, TGetListInput>
    where TEntity : class, IEntity
{
    Task<TGetOutputDto> GetAsync(TKey id, CancellationToken cancellationToken = default);
}

public interface
    IReadOnlyAppService<TEntity, TOutputDto> : IReadOnlyAppService<TEntity, TOutputDto, TOutputDto,
    IRequest> where TEntity : class, IEntity
{
}