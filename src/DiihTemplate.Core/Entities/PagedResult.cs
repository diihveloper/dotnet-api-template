using DiihTemplate.Core.Commons;
using DiihTemplate.Core.Dtos;

namespace DiihTemplate.Core.Entities;

public class PagedResult<T> : ListResult<T>, IPagedResult<T>
{
    public long Count { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public PagedResult()
    {
    }

    protected PagedResult(List<T> items)
    {
        Items = items;
        Count = items.Count;
    }

    protected PagedResult(List<T> items, long count) : base(items)
    {
        Count = count;
    }

    public PagedResult(List<T> list, int count, int page, int perPage) : this(list, count)
    {
        Page = page < 1 ? 1 : page;
        PageSize = perPage < 1 ? 1 : perPage;
    }


    public static PagedResult<T> Create<TInput>(List<T> items)
    {
        return new PagedResult<T>(items);
    }

    public static PagedResult<T> Create<TInput>(List<T> items, TInput input, long count)
    {
        if (input is IPagedRequest pagedRequest)
        {
            return new PagedResult<T>(items, count)
            {
                Page = pagedRequest.Page,
                PageSize = pagedRequest.PageSize
            };
        }

        return new PagedResult<T>(items, count);
    }
}