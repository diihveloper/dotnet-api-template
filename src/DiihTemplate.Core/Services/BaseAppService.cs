using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Repositories;
using MapsterMapper;

namespace DiihTemplate.Core.Services;

public abstract class BaseAppService<TEntity, TGetOutputDto, TGetListOutputDto, TGetListInput>
    where TEntity : class, IEntity
{
    protected virtual int PageSize { get; set; } = 10;
    protected IMapper Mapper { get; }
    protected IReadOnlyRepository<TEntity> ReadOnlyRepository { get; }

    protected BaseAppService(IMapper mapper, IReadOnlyRepository<TEntity> repository)
    {
        Mapper = mapper;
        ReadOnlyRepository = repository;
    }

    protected virtual TGetOutputDto MapToOutputDto(TEntity entity)
    {
        return Mapper.Map<TGetOutputDto>(entity);
    }

    protected virtual List<TGetListOutputDto> MapToListOutputDtos(List<TEntity> entities)
    {
        return Mapper.Map<List<TGetListOutputDto>>(entities);
    }

    protected virtual TGetListOutputDto MapToListOutputDto(TEntity entity)
    {
        return Mapper.Map<TGetListOutputDto>(entity);
    }

    protected virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, TGetListInput input)
    {
        if (input is not ISearchRequest searchRequest) return query;
        return query.Search(searchRequest.Search);
    }

    protected virtual Task<IQueryable<TEntity>> ApplyFilterAsync(IQueryable<TEntity> query, TGetListInput input)
    {
        if (input is not ISearchRequest searchRequest) return Task.FromResult(query);
        return Task.FromResult(query.Search(searchRequest.Search));
    }

    protected virtual IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, TGetListInput input)
    {
        return query;
    }

    protected virtual IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, TGetListInput input)
    {
        if (input is not IPagedRequest pagedRequest) return query;
        pagedRequest = pagedRequest.Validate(PageSize);
        return query.PageBy(pagedRequest.Page, pagedRequest.PageSize);
    }
}