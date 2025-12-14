using System.Linq.Expressions;
using MapsterMapper;
using DiihTemplate.Core.Commons;
using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiihTemplate.Core.Services;

public class
    ReadOnlyAppService<TEntity, TGetOutputDto, TGetListOutputDto, TGetListInput> : BaseAppService<TEntity, TGetOutputDto, TGetListOutputDto, TGetListInput>, IReadOnlyAppService<TEntity,
    TGetOutputDto, TGetListOutputDto, TGetListInput> where TEntity : class, IEntity
{
    public ReadOnlyAppService(IReadOnlyRepository<TEntity> repository, IMapper mapper) : base(mapper, repository)
    {
    }

    public async Task<TGetOutputDto?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entity = await ReadOnlyRepository.FindAsync(predicate, cancellationToken);
        if (entity == null) return default;
        return MapToOutputDto(entity);
    }

    public async Task<TGetOutputDto> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entity = await ReadOnlyRepository.GetAsync(predicate, cancellationToken);
        return MapToOutputDto(entity);
    }

    public virtual async Task<IPagedResult<TGetListOutputDto>> GetPagedAsync(TGetListInput input,
        CancellationToken cancellationToken = default)
    {
        var query = await ReadOnlyRepository.GetQueryableAsync();

        query = await ApplyFilterAsync(query, input);
        var count = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, input);
        query = ApplyPaging(query, input);

        var entities = await query.ToListAsync(cancellationToken);
        var dtos = MapToListOutputDtos(entities);
        return PagedResultDto<TGetListOutputDto>.Create(dtos, input, count);
    }

    public virtual async Task<IListResult<TGetListOutputDto>> GetAllAsync(TGetListInput input,
        CancellationToken cancellationToken = default)
    {
        var query = await ReadOnlyRepository.GetQueryableAsync();

        query = await ApplyFilterAsync(query, input);
        query = ApplySorting(query, input);

        var entities = await query.ToListAsync(cancellationToken);
        var dtos = MapToListOutputDtos(entities);
        return new ListResultDto<TGetListOutputDto>(dtos);
    }

    public virtual async Task<IListResult<TGetListOutputDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = await ReadOnlyRepository.GetQueryableAsync();
        var entities = await query.ToListAsync(cancellationToken);
        var dtos = MapToListOutputDtos(entities);
        return new ListResultDto<TGetListOutputDto>(dtos);
    }
}

public class ReadOnlyAppService<TEntity, TOutputDto> : ReadOnlyAppService<TEntity, TOutputDto, TOutputDto, IRequest>
    where TEntity : class, IEntity
{
    public ReadOnlyAppService(IReadOnlyRepository<TEntity> repository, IMapper mapper) : base(repository, mapper)
    {
    }
}

public class ReadOnlyAppService<TEntity, TKey, TGetOutputDto, TGetListOutputDto, TGetListInput> :
    ReadOnlyAppService<TEntity, TGetOutputDto, TGetListOutputDto, TGetListInput>,
    IReadOnlyAppService<TEntity, TKey, TGetOutputDto, TGetListOutputDto, TGetListInput>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    new IReadOnlyRepository<TEntity, TKey> ReadOnlyRepository { get; }

    public ReadOnlyAppService(IReadOnlyRepository<TEntity, TKey> repository, IMapper mapper) : base(repository, mapper)
    {
        ReadOnlyRepository = repository;
    }

    public virtual async Task<TGetOutputDto> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await ReadOnlyRepository.GetAsync(id, cancellationToken);
        return MapToOutputDto(entity);
    }
}