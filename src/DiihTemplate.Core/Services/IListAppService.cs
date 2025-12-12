using DiihTemplate.Core.Commons;

namespace DiihTemplate.Core.Services;

public interface IListAppService<TOutputDto, TGetListInput>
{
    Task<IPagedResult<TOutputDto>> GetPagedAsync(TGetListInput input,
        CancellationToken cancellationToken = default);

    Task<IListResult<TOutputDto>> GetAllAsync(TGetListInput input,
        CancellationToken cancellationToken = default);

    Task<IListResult<TOutputDto>> GetAllAsync(CancellationToken cancellationToken = default);
}