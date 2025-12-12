using AutoMapper;
using DiihTemplate.Core.Commons;
using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiihTemplate.Core.Services;

public class
    ListAppService<TEntity, TGetListOutputDto, TGetListInput> :
    BaseAppService<TEntity, TGetListOutputDto, TGetListOutputDto, TGetListInput>,
    IListAppService<TGetListOutputDto, TGetListInput> where TEntity : class, IEntity
{
    protected ListAppService(IMapper mapper, IReadOnlyRepository<TEntity> repository) : base(mapper, repository)
    {
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