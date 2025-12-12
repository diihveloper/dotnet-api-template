using DiihTemplate.Core.Commons;
using Microsoft.AspNetCore.Mvc;

namespace DiihTemplate.Core.Controllers;

public interface IReadOnlyController<TKey, TGetOutputDto, TGetListOutputDto, TGetListInput> :
    IReadOnlyController<TGetListOutputDto, TGetListInput>,
    ISingleReadOnlyController<TKey, TGetOutputDto>
{
}

public interface IReadOnlyController<TGetListOutputDto, TGetListInput>
{
    Task<ActionResult<IPagedResult<TGetListOutputDto>>> GetPageAsync(TGetListInput request);
}