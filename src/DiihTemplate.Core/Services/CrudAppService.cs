using MapsterMapper;
using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Repositories;

namespace DiihTemplate.Core.Services;

public class
    CrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateInput, TUpdateInput> :
    ReadOnlyAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput>,
    ICrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateInput,
        TUpdateInput> where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    protected IRepository<TEntity, TKey> Repository { get; }

    public CrudAppService(IRepository<TEntity, TKey> repository, IMapper mapper) : base(repository, mapper)
    {
        Repository = repository;
    }

    protected virtual TEntity MapToEntity(TCreateInput input)
    {
        return Mapper.Map<TEntity>(input);
    }

    protected virtual TEntity MapToEntity(TUpdateInput input)
    {
        return Mapper.Map<TEntity>(input);
    }

    protected virtual void MapToEntity(TUpdateInput input, TEntity entity)
    {
        Mapper.Map(input, entity);
    }


    public virtual async Task<TOutputDto?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.FindAsync(id, cancellationToken);
        return entity == null ? default : MapToOutputDto(entity);
    }

    public virtual async Task<TOutputDto> CreateAsync(TCreateInput input, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(input);
        entity = await Repository.InsertAsync(entity, true, cancellationToken);
        return MapToOutputDto(entity);
    }

    public virtual async Task<TOutputDto> UpdateAsync(TKey id, TUpdateInput input,
        CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetAsync(id, cancellationToken);
        MapToEntity(input, entity);
        entity = await Repository.UpdateAsync(entity, true, cancellationToken);
        return MapToOutputDto(entity);
    }

    public virtual Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return Repository.DeleteAsync(id, true, cancellationToken);
    }
}

public class
    CrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateUpdateInput> :
    CrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateUpdateInput, TCreateUpdateInput>,
    ICrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateUpdateInput>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    public CrudAppService(IRepository<TEntity, TKey> repository, IMapper mapper) : base(repository, mapper)
    {
    }
}

public class
    CrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput> :
    CrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TEntity>,
    ICrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput> where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public CrudAppService(IRepository<TEntity, TKey> repository, IMapper mapper) : base(repository, mapper)
    {
    }
}

public class
    CrudAppService<TEntity, TKey, TOutputDto, TGetListInput> :
    CrudAppService<TEntity, TKey, TOutputDto, TOutputDto, TGetListInput, TOutputDto>,
    ICrudAppService<TEntity, TKey, TOutputDto, TGetListInput>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    public CrudAppService(IRepository<TEntity, TKey> repository, IMapper mapper) : base(repository, mapper)
    {
    }
}

public class CrudAppService<TEntity, TKey, TDto> : CrudAppService<TEntity, TKey, TDto, TDto, IPagedRequest>,
    ICrudAppService<TEntity, TKey, TDto> where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    public CrudAppService(IRepository<TEntity, TKey> repository, IMapper mapper) : base(repository, mapper)
    {
    }
}