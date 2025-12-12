using DiihTemplate.Core.Commons;

namespace DiihTemplate.Core.Dtos;

public class PagedResultDto<T> : ListResultDto<T>, IPagedResult<T>
{
    public long Count { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public PagedResultDto()
    {

    }
    protected PagedResultDto(List<T> items)
    {
        Items = items;
        Count = items.Count;
    }

    protected PagedResultDto(List<T> items, long count) : base(items)
    {
        Count = count;
    }


    public static PagedResultDto<T> Create<TInput>(List<T> items)
    {
        return new PagedResultDto<T>(items);
    }

    public static PagedResultDto<T> Create<TInput>(List<T> items, TInput input, long count)
    {
        if (input is IPagedRequest pagedRequest)
        {
            return new PagedResultDto<T>(items, count)
            {
                Page = pagedRequest.Page,
                PageSize = pagedRequest.PageSize
            };
        }

        return new PagedResultDto<T>(items, count);
    }
}