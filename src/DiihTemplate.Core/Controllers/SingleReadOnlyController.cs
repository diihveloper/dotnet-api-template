using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiihTemplate.Core.Controllers;

public abstract class
    SingleReadOnlyController<TEntity, TKey, TGetOutputDto> : ISingleReadOnlyController<TKey, TGetOutputDto>
    where TEntity : class, IEntity
{
    IReadOnlyAppService<TEntity, TKey, TGetOutputDto, TGetOutputDto, PagedRequest> _appService;

    protected SingleReadOnlyController(
        IReadOnlyAppService<TEntity, TKey, TGetOutputDto, TGetOutputDto, PagedRequest> appService)
    {
        _appService = appService;
    }


    public async Task<ActionResult<TGetOutputDto>> GetAsync(TKey id)
    {
        return await _appService.GetAsync(id);
    }
}