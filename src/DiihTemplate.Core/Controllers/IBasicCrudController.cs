using Microsoft.AspNetCore.Mvc;

namespace DiihTemplate.Core.Controllers;

public interface
    IBasicCrudController<TKey, TGetOutputDto, TGetListOutputDto, TGetListInput, TCreateInput,
        TUpdateInput> : ISingleReadOnlyController<TKey, TGetOutputDto>,
    IReadOnlyController<TGetListOutputDto, TGetListInput>
{
    Task<ActionResult<TGetOutputDto>> CreateAsync(TCreateInput input);
    Task<ActionResult> UpdateAsync(TKey id, TUpdateInput input);
    Task<ActionResult> DeleteAsync(TKey id);
}

public interface IBasicCrudController<TKey, TGetOutputDto, TGetListOutputDto, TGetListInput, TCreateUpdateInput> :
    IBasicCrudController<TKey, TGetOutputDto, TGetListOutputDto, TGetListInput, TCreateUpdateInput, TCreateUpdateInput>
{
}

public interface IBasicCrudController<TKey, TDto, TGetListOutputDto, TGetListInput> :
    IBasicCrudController<TKey, TDto, TGetListOutputDto, TGetListInput, TDto>
{
}

public interface IBasicCrudController<TKey, TDto, TGetListInput> :
    IBasicCrudController<TKey, TDto, TDto, TGetListInput, TDto>
{
}