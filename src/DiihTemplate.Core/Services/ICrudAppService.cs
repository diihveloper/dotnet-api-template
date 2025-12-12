using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Entities;

namespace DiihTemplate.Core.Services;

public interface
    ICrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateInput, TUpdateInput> :
    IReadOnlyAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    Task<TOutputDto?> FindAsync(TKey id, CancellationToken cancellationToken = default);
    Task<TOutputDto> CreateAsync(TCreateInput input, CancellationToken cancellationToken = default);
    Task<TOutputDto> UpdateAsync(TKey id, TUpdateInput input, CancellationToken cancellationToken = default);
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}

public interface
    ICrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateUpdateInput> : ICrudAppService<
    TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateUpdateInput,
    TCreateUpdateInput>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
}

public interface
    ICrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput> :
    ICrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TEntity>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
}

public interface
    ICrudAppService<TEntity, TKey, TDto, TGetListInput> : ICrudAppService<TEntity, TKey, TDto,
    TDto, TGetListInput, TDto>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
}

public interface
    ICrudAppService<TEntity, TKey, TDto> : ICrudAppService<TEntity, TKey, TDto, TDto, IPagedRequest>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
}