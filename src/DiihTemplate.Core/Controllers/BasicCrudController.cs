using Microsoft.AspNetCore.Mvc;
using DiihTemplate.Core.Commons;
using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Services;

namespace DiihTemplate.Core.Controllers;

public abstract class BasicCrudController<TEntity, TKey, TGetOutputDto, TGetListOutputDto, TGetListInput, TCreateInput,
    TUpdateInput> : ControllerBase,
    IBasicCrudController<TKey, TGetOutputDto, TGetListOutputDto, TGetListInput, TCreateInput,
        TUpdateInput> where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    protected ICrudAppService<TEntity, TKey, TGetOutputDto, TGetListOutputDto, TGetListInput, TCreateInput,
            TUpdateInput>
        Service;

    protected BasicCrudController(
        ICrudAppService<TEntity, TKey, TGetOutputDto, TGetListOutputDto, TGetListInput, TCreateInput, TUpdateInput>
            service)
    {
        Service = service;
    }

    protected virtual void CheckGetPermission(TKey id)
    {
        // throw NotAuthorizedException()
    }

    protected virtual void CheckCreatePermission(TCreateInput input)
    {
        // throw NotAuthorizedException()
    }

    protected virtual void CheckUpdatePermission(TKey id, TUpdateInput input)
    {
        // throw NotAuthorizedException()
    }

    protected virtual void CheckDeletePermission(TKey id)
    {
        // throw NotAuthorizedException()
    }

    protected virtual void CheckListPermission(TGetListInput request)
    {
        // throw NotAuthorizedException()
    }

    [HttpPost]
    public virtual async Task<ActionResult<TGetOutputDto>> CreateAsync(TCreateInput input)
    {
        CheckCreatePermission(input);
        return await Service.CreateAsync(input);
    }

    [HttpGet]
    public virtual async Task<ActionResult<IPagedResult<TGetListOutputDto>>> GetPageAsync(
        [FromQuery] TGetListInput request)
    {
        CheckListPermission(request);
        var result = await Service.GetPagedAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TGetOutputDto>> GetAsync(TKey id)
    {
        CheckGetPermission(id);
        return await Service.GetAsync(id);
    }


    [HttpPut("{id}")]
    public virtual async Task<ActionResult> UpdateAsync(TKey id, TUpdateInput input)
    {
        CheckUpdatePermission(id, input);
        await Service.UpdateAsync(id, input);
        return Ok();
    }

    [HttpDelete("{id}")]
    public virtual async Task<ActionResult> DeleteAsync(TKey id)
    {
        CheckDeletePermission(id);
        await Service.DeleteAsync(id);
        return Ok();
    }
}

public abstract class
    BasicCrudController<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateUpdateInput> :
    BasicCrudController<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateUpdateInput,
        TCreateUpdateInput>,
    IBasicCrudController<TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateUpdateInput>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    protected BasicCrudController(
        ICrudAppService<TEntity, TKey, TOutputDto, TOutputListDto, TGetListInput, TCreateUpdateInput> service) :
        base(service)
    {
    }
}

public abstract class BasicCrudController<TEntity, TKey, TDto, TGetListOutputDto, TGetListInput> :
    BasicCrudController<TEntity, TKey, TDto, TGetListOutputDto, TGetListInput, TDto>,
    IBasicCrudController<TKey, TDto, TGetListOutputDto, TGetListInput>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    protected BasicCrudController(
        ICrudAppService<TEntity, TKey, TDto, TGetListOutputDto, TGetListInput, TDto> service) : base(service)
    {
    }
}

public abstract class BasicCrudController<TEntity, TKey, TDto, TGetListInput> :
    BasicCrudController<TEntity, TKey, TDto, TDto, TGetListInput, TDto>,
    IBasicCrudController<TKey, TDto, TGetListInput>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    protected BasicCrudController(
        ICrudAppService<TEntity, TKey, TDto, TGetListInput> service) : base(service)
    {
    }
}

public abstract class BasicCrudController<TEntity, TKey, TDto> :
    BasicCrudController<TEntity, TKey, TDto, TDto, IPagedRequest>,
    IBasicCrudController<TKey, TDto, IPagedRequest>
    where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    protected BasicCrudController(
        ICrudAppService<TEntity, TKey, TDto, IPagedRequest> service) : base(service)
    {
    }
}