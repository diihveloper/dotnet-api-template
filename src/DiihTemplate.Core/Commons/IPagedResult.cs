namespace DiihTemplate.Core.Commons;

public interface IPagedResult<T> : IListResult<T>, IHasCount
{
    int Page { get; set; }
    int PageSize { get; set; }
}