using DiihTemplate.Core.Commons;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiihTemplate.Core.Controllers;

public abstract class
    ReadOnlyController<TEntity, TGetListOutputDto, TGetListInput> : ControllerBase,
    IReadOnlyController<TGetListOutputDto, TGetListInput> where TEntity : class, IEntity
{
    IListAppService<TGetListOutputDto, TGetListInput> _appService;

    protected ReadOnlyController(IListAppService<TGetListOutputDto, TGetListInput> appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<ActionResult<IPagedResult<TGetListOutputDto>>> GetPageAsync([FromQuery] TGetListInput request)
    {
        var result = await _appService.GetPagedAsync(request);
        return Ok(result);
    }
}

public abstract class
    ReadOnlyController<TEntity, TKey, TGetOutputDto, TGetListOutputDto, TGetListInput> :
    ReadOnlyController<TEntity, TGetListOutputDto, TGetListInput>,
    IReadOnlyController<TKey, TGetOutputDto, TGetListOutputDto, TGetListInput> where TEntity : class, IEntity
{
    IReadOnlyAppService<TEntity, TKey, TGetOutputDto, TGetListOutputDto, TGetListInput> _appService;


    protected ReadOnlyController(
        IReadOnlyAppService<TEntity, TKey, TGetOutputDto, TGetListOutputDto, TGetListInput> appService)
        : base(appService)
    {
        _appService = appService;
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<TGetOutputDto>> GetAsync(TKey id)
    {
        var result = await _appService.GetAsync(id);
        return Ok(result);
    }
}