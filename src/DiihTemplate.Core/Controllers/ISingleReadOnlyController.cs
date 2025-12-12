using Microsoft.AspNetCore.Mvc;

namespace DiihTemplate.Core.Controllers;

public interface ISingleReadOnlyController<TKey, TGetOutputDto>
{
    Task<ActionResult<TGetOutputDto>> GetAsync(TKey id);
}